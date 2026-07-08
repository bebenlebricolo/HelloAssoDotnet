using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.Api.Organizations;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Utils;

namespace HelloAssoDotnet.Client;

/// <inheritdoc cref="IUsersClient" />
internal sealed class UsersClient : HelloAssoSubClient, IUsersClient
{
    public UsersClient(IHelloAssoClientContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Result<List<OrganizationLightModel>>> GetMyOrganizationsAsync(AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<List<OrganizationLightModel>>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/users/me/organizations";
        var request = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<List<OrganizationLightModel>>(request, Context.Logger, cancellationToken);
    }
}
