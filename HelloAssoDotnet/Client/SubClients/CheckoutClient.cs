using HelloAssoDotnet.Models.HelloAssoApi.Auth;
using HelloAssoDotnet.Models.HelloAssoApi.Checkout;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Utils;

namespace HelloAssoDotnet.Client.SubClients;

/// <inheritdoc cref="ICheckoutClient" />
internal sealed class CheckoutClient : HelloAssoSubClient, ICheckoutClient
{
    public CheckoutClient(IHelloAssoClientContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Result<CheckoutIntentResponse>> GetIntentAsync(long checkoutIntentId, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<CheckoutIntentResponse>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/checkout-intents/{checkoutIntentId}";
        var request = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<CheckoutIntentResponse>(request, Context.Logger, cancellationToken);
    }
}
