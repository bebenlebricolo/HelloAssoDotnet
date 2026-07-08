using HelloAssoDotnet.Models.HelloAssoApi.Auth;
using HelloAssoDotnet.Models.HelloAssoApi.Order;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Utils;
using Microsoft.Extensions.Logging;

namespace HelloAssoDotnet.Client.SubClients;

/// <inheritdoc cref="IOrdersClient" />
internal sealed class OrdersClient : HelloAssoSubClient, IOrdersClient
{
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
    public async Task<Result<ListOrdersResponse>> ListForOrganizationAsync(ListOrdersRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<ListOrdersResponse>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/orders";
        url = HelloAssoQuery.BuildOrders(url, request);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<ListOrdersResponse>(requestMessage, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<OrderDetails> ListAllForOrganizationAsync(ListOrdersRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        return HelloAssoPager.PageAllAsync<ListOrdersResponse, OrderDetails>(
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

        List<Task<HttpResponseMessage>> tasks = new List<Task<HttpResponseMessage>>();
        foreach (var ticketUrl in ticketsUrls)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, ticketUrl)
                .WithBearer(accessToken.Value!)
                .WithUserAgent(Context.Config)
                .WithPdfAccept();
            var task = Context.HttpClient.SendAsync(request, cancellationToken);
            tasks.Add(task);
        }

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
