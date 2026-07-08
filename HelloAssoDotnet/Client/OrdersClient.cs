using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.Api.Order;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Utils;
using Microsoft.Extensions.Logging;

namespace HelloAssoDotnet.Client;

/// <inheritdoc cref="IOrdersClient" />
internal sealed class OrdersClient : HelloAssoSubClient, IOrdersClient
{
    /// <summary>
    /// Upper bound on parallel ticket-PDF downloads. Downloading every ticket of a large order at once would
    /// hammer HelloAsso and risk being rate-limited/flagged, so the fan-out is capped.
    /// </summary>
    private const int MaxConcurrentTicketDownloads = 4;

    public OrdersClient(IHelloAssoClientContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Result<OrderDetails>> GetAsync(int orderId, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<OrderDetails>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/orders/{orderId}";
        var request = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<OrderDetails>(request, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<PaginatedResponse<OrderDetails>>> ListForOrganizationAsync(ListOrdersRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<PaginatedResponse<OrderDetails>>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/orders";
        url = HelloAssoQuery.BuildOrders(url, request);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<PaginatedResponse<OrderDetails>>(requestMessage, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<OrderDetails> ListAllForOrganizationAsync(ListOrdersRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        return HelloAssoPager.PageAllAsync<PaginatedResponse<OrderDetails>, OrderDetails>(
            (continuationToken, ct) =>
            {
                var pageRequest = request with { ContinuationToken = continuationToken };
                return ListForOrganizationAsync(pageRequest, tokens, ct);
            },
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<List<Stream>>> GetEventTicketsPdfAsync(PaymentResponse payment, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<List<Stream>>.FromError(accessToken.Error);
        }

        var orderDetails = await GetAsync(payment.Order.Id, tokens, cancellationToken);
        if (!orderDetails.IsOk)
        {
            Context.Logger.LogError($"Failed to get order details for order {payment.Order.Id}");
            Context.Logger.LogError("Error was {error}", orderDetails.Error);
            return Result<List<Stream>>.FromError(orderDetails.Error);
        }

        List<string> ticketsUrls = orderDetails.Value!.Items!.Select(ticket => ticket.TicketUrl).ToList()!;
        List<Stream> ticketsPdfs = new List<Stream>();

        // Download the tickets with a bounded degree of parallelism. The throttler gates concurrent requests
        // while Select preserves order, so tasks[i] still matches ticketsUrls[i] below.
        using var throttler = new SemaphoreSlim(MaxConcurrentTicketDownloads);

        async Task<HttpResponseMessage> DownloadTicketAsync(string ticketUrl)
        {
            await throttler.WaitAsync(cancellationToken);
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, ticketUrl)
                    .WithBearer(accessToken.Value!)
                    .WithUserAgent(Context.Config)
                    .WithPdfAccept();
                return await Context.HttpClient.SendAsync(request, cancellationToken);
            }
            finally
            {
                throttler.Release();
            }
        }

        List<Task<HttpResponseMessage>> tasks = ticketsUrls.Select(DownloadTicketAsync).ToList();

        // Wait all tasks
        await Task.WhenAll(tasks).ConfigureAwait(false);
        var errors = new Dictionary<string, HttpResponseMessage>();
        for (int i = 0 ; i < tasks.Count ; i++)
        {
            var result = tasks[i].Result;
            if (!result.IsSuccessStatusCode)
            {
                errors.Add(ticketsUrls[i], result);
            }
            else
            {
                ticketsPdfs.Add(await result.Content.ReadAsStreamAsync(cancellationToken));
            }
        }

        if (errors.Count > 0)
        {
            Context.Logger.LogError("Encountered issues while downloading tickets:");
            foreach (var error in errors)
            {
                Context.Logger.LogError("For ticket : {ticket}",error.Key);
                Context.Logger.LogError(error.Value.ToString());
                Context.Logger.LogError("HttpContent was : {content}", await error.Value.Content.ReadAsStringAsync(cancellationToken));
            }

            return Result<List<Stream>>.FromError(Errors.ServerError);
        }
        return  Result<List<Stream>>.Ok(ticketsPdfs);
    }
}
