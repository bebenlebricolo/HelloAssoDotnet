using HelloAssoDotnet.Models.HelloAssoApi.Auth;
using HelloAssoDotnet.Models.HelloAssoApi.Partners;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Utils;

namespace HelloAssoDotnet.Client.SubClients;

/// <inheritdoc cref="IPartnersClient" />
internal sealed class PartnersClient : HelloAssoSubClient, IPartnersClient
{
    public PartnersClient(IHelloAssoClientContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Result<PartnerInfo>> GetMeAsync(AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<PartnerInfo>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/partners/me";
        var request = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<PartnerInfo>(request, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<ListOrganizationsResponse>> GetOrganizationsAsync(string? continuationToken = null, int? pageSize = null, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<ListOrganizationsResponse>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/partners/me/organizations";
        uint queriesCount = 0;
        if (pageSize != null)
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, pageSize.ToString()!, "pageSize", ref queriesCount);
        }
        if (continuationToken != null)
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, continuationToken, "continuationToken", ref queriesCount);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<ListOrganizationsResponse>(request, Context.Logger, cancellationToken);
    }
}
