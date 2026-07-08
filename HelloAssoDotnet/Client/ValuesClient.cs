using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.Api.Forms;
using HelloAssoDotnet.Models.Api.Values;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Utils;

namespace HelloAssoDotnet.Client;

/// <inheritdoc cref="IValuesClient" />
internal sealed class ValuesClient : HelloAssoSubClient, IValuesClient
{
    public ValuesClient(IHelloAssoClientContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public Task<Result<List<CompanyLegalStatus>>> GetCompanyLegalStatusesAsync(AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        return GetValuesAsync<CompanyLegalStatus>("values/company-legal-status", tokens, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<List<OrganizationCategory>>> GetOrganizationCategoriesAsync(AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        return GetValuesAsync<OrganizationCategory>("values/organization/categories", tokens, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<List<PublicTag>>> GetTagsAsync(AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        return GetValuesAsync<PublicTag>("values/tags", tokens, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<List<FormSubType>>> GetFormSubTypesAsync(FormType formType, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        return GetValuesAsync<FormSubType>($"values/form/{formType}/types", tokens, cancellationToken);
    }

    private async Task<Result<List<T>>> GetValuesAsync<T>(string relativePath, AuthTokens? tokens, CancellationToken cancellationToken) where T : class
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<List<T>>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/{relativePath}";
        var request = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<List<T>>(request, Context.Logger, cancellationToken);
    }
}
