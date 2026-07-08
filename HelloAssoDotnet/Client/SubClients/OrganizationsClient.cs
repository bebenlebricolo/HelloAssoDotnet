using HelloAssoDotnet.Models.HelloAssoApi.Auth;
using HelloAssoDotnet.Models.HelloAssoApi.Organizations;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Utils;

namespace HelloAssoDotnet.Client.SubClients;

/// <inheritdoc cref="IOrganizationsClient" />
internal sealed class OrganizationsClient : HelloAssoSubClient, IOrganizationsClient
{
    public OrganizationsClient(IHelloAssoClientContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Result<OrganizationDetails>> GetAsync(AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<OrganizationDetails>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}";
        var request = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<OrganizationDetails>(request, Context.Logger, cancellationToken);
    }
}
