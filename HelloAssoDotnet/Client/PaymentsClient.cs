using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.Api.Payment;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Utils;
using Microsoft.Extensions.Logging;

namespace HelloAssoDotnet.Client;

/// <inheritdoc cref="IPaymentsClient" />
internal sealed class PaymentsClient : HelloAssoSubClient, IPaymentsClient
{
    public PaymentsClient(IHelloAssoClientContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Result<SearchPaymentResponse>> SearchAsync(SearchPaymentsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<SearchPaymentResponse>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/payments";
        url = HelloAssoQuery.BuildPayments(url, request);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<SearchPaymentResponse>(requestMessage, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<PaymentResponse> SearchAllAsync(SearchPaymentsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        return HelloAssoPager.PageAllAsync<SearchPaymentResponse, PaymentResponse>(
            (continuationToken, ct) =>
            {
                var pageRequest = request with { ContinuationToken = continuationToken };
                return SearchAsync(pageRequest, tokens, ct);
            },
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<SearchPaymentResponse>> SearchForUserAsync(string email, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var request = new SearchPaymentsRequest
        {
            UserSearchKey = email,
            States = new List<PaymentState> { PaymentState.Authorized },
        };
        return SearchAsync(request, tokens, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<PaymentResponse>> GetAsync(int paymentId, bool withFailedRefundOperations = false, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<PaymentResponse>.FromError(accessToken.Error);
        }

        string url = $"{Context.BaseUri}/payments/{paymentId}";
        if (withFailedRefundOperations)
        {
            url += $"?withFailedRefundOperation=true";
        }

        var request = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<PaymentResponse>(request, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<Stream>> GetReceiptPdfAsync(PaymentResponse payment, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<Stream>.FromError(accessToken.Error);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, payment.PaymentReceiptUrl)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithPdfAccept();

        var response = await Context.HttpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            Context.Logger.LogError($"Failed to get receipt PDF details for order {payment.PaymentReceiptUrl}");
            Context.Logger.LogError("Error was {error}", response.ToString());
            Context.Logger.LogError("HttpContent was : {content}", await response.Content.ReadAsStringAsync(cancellationToken));
            return Result<Stream>.FromHttpResponse(response);
        }

        return Result<Stream>.Ok(await response.Content.ReadAsStreamAsync(cancellationToken));
    }
}
