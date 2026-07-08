using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.Api.Forms;
using HelloAssoDotnet.Models.Api.Order;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Utils;

namespace HelloAssoDotnet.Client;

/// <inheritdoc cref="IFormsClient" />
internal sealed class FormsClient : HelloAssoSubClient, IFormsClient
{
    public FormsClient(IHelloAssoClientContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Result<PaginatedResponse<FormLightModel>>> ListAsync(ListOrganizationFormsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<PaginatedResponse<FormLightModel>>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/forms";
        url = HelloAssoQuery.BuildForms(url, request);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<PaginatedResponse<FormLightModel>>(requestMessage, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<FormLightModel> ListAllAsync(ListOrganizationFormsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        return HelloAssoPager.PageAllAsync<PaginatedResponse<FormLightModel>, FormLightModel>(
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

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/formTypes";
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
    public async Task<Result<PaginatedResponse<OrderItem>>> GetItemsAsync(FormType formType, string formSlug, ListItemsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<PaginatedResponse<OrderItem>>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/forms/{formType}/{formSlug}/items";
        url = HelloAssoQuery.BuildItems(url, request);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<PaginatedResponse<OrderItem>>(requestMessage, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<PaginatedResponse<OrderDetails>>> GetOrdersAsync(FormType formType, string formSlug, ListOrdersRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<PaginatedResponse<OrderDetails>>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/forms/{formType}/{formSlug}/orders";
        url = HelloAssoQuery.BuildOrders(url, request);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<PaginatedResponse<OrderDetails>>(requestMessage, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<PaginatedResponse<PaymentResponse>>> GetPaymentsAsync(FormType formType, string formSlug, SearchPaymentsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<PaginatedResponse<PaymentResponse>>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/forms/{formType}/{formSlug}/payments";
        url = HelloAssoQuery.BuildPayments(url, request);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<PaginatedResponse<PaymentResponse>>(requestMessage, Context.Logger, cancellationToken);
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
