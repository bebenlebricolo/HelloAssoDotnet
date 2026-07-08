using HelloAssoDotnet.Models.HelloAssoApi.Auth;
using HelloAssoDotnet.Models.HelloAssoApi.Forms;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Utils;

namespace HelloAssoDotnet.Client.SubClients;

/// <inheritdoc cref="IFormsClient" />
internal sealed class FormsClient : HelloAssoSubClient, IFormsClient
{
    public FormsClient(IHelloAssoClientContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Result<ListOrganizationFormsResponse>> ListAsync(ListOrganizationFormsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<ListOrganizationFormsResponse>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/forms";
        url = HelloAssoQuery.BuildForms(url, request);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<ListOrganizationFormsResponse>(requestMessage, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<FormLightModel> ListAllAsync(ListOrganizationFormsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        return HelloAssoPager.PageAllAsync<ListOrganizationFormsResponse, FormLightModel>(
            (continuationToken, ct) =>
            {
                var pageRequest = request with { ContinuationToken = continuationToken };
                return ListAsync(pageRequest, tokens, ct);
            },
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<FormDetails>> GetPublicDetailsAsync(string formSlug, FormType formType, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<FormDetails>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/forms/{formType}/{formSlug}/public";
        var request = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<FormDetails>(request, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<List<FormType>>> GetTypesAsync(IEnumerable<PublicationState>? states = null, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<List<FormType>>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/formtypes";
        if (states != null)
        {
            uint queriesCount = 0;
            foreach (var state in states)
            {
                url = HelloAssoHttpExtensions.AddQueryToUrl(url, state.ToString(), "states", ref queriesCount);
            }
        }

        var request = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<List<FormType>>(request, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<ListItemsResponse>> GetItemsAsync(FormType formType, string formSlug, ListItemsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<ListItemsResponse>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/forms/{formType}/{formSlug}/items";
        url = HelloAssoQuery.BuildItems(url, request);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<ListItemsResponse>(requestMessage, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<ListOrdersResponse>> GetOrdersAsync(FormType formType, string formSlug, ListOrdersRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<ListOrdersResponse>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/forms/{formType}/{formSlug}/orders";
        url = HelloAssoQuery.BuildOrders(url, request);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<ListOrdersResponse>(requestMessage, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<SearchPaymentResponse>> GetPaymentsAsync(FormType formType, string formSlug, SearchPaymentsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<SearchPaymentResponse>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/forms/{formType}/{formSlug}/payments";
        url = HelloAssoQuery.BuildPayments(url, request);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<SearchPaymentResponse>(requestMessage, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<FormStats>> GetStatsAsync(FormType formType, string formSlug, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<FormStats>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/forms/{formType}/{formSlug}/stats";
        var request = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<FormStats>(request, Context.Logger, cancellationToken);
    }
}
