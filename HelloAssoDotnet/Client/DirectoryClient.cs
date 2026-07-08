using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.Api.Forms;
using HelloAssoDotnet.Models.Api.Organizations;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Utils;

namespace HelloAssoDotnet.Client;

/// <inheritdoc cref="IDirectoryClient" />
internal sealed class DirectoryClient : HelloAssoSubClient, IDirectoryClient
{
    public DirectoryClient(IHelloAssoClientContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Result<PaginatedResponse<FormLightModel>>> SearchFormsAsync(DirectoryFormsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<PaginatedResponse<FormLightModel>>.FromError(accessToken.Error);
        }

        var url = BuildDirectoryUrl($"{Context.BaseUri}/directory/forms", request.PageSize, request.ContinuationToken);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept()
            .WithJsonBody(request.Filters);

        return await Context.HttpClient.SendJsonAsync<PaginatedResponse<FormLightModel>>(requestMessage, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<PaginatedResponse<OrganizationLightModel>>> SearchOrganizationsAsync(DirectoryOrganizationsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<PaginatedResponse<OrganizationLightModel>>.FromError(accessToken.Error);
        }

        var url = BuildDirectoryUrl($"{Context.BaseUri}/directory/organizations", request.PageSize, request.ContinuationToken);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept()
            .WithJsonBody(request.Filters);

        return await Context.HttpClient.SendJsonAsync<PaginatedResponse<OrganizationLightModel>>(requestMessage, Context.Logger, cancellationToken);
    }

    private static string BuildDirectoryUrl(string url, int? pageSize, string? continuationToken)
    {
        uint queriesCount = 0;
        if (pageSize != null)
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, pageSize.ToString()!, "pageSize", ref queriesCount);
        }

        if (continuationToken != null)
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, continuationToken, "continuationToken", ref queriesCount);
        }

        return url;
    }
}
