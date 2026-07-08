using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.Api.Order;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Utils;

namespace HelloAssoDotnet.Client;

/// <inheritdoc cref="IItemsClient" />
internal sealed class ItemsClient : HelloAssoSubClient, IItemsClient
{
    public ItemsClient(IHelloAssoClientContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Result<OrderItem>> GetAsync(int itemId, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<OrderItem>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/items/{itemId}";
        var request = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<OrderItem>(request, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<ListItemsResponse>> ListForOrganizationAsync(ListItemsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<ListItemsResponse>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/items";
        url = HelloAssoQuery.BuildItems(url, request);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        return await Context.HttpClient.SendJsonAsync<ListItemsResponse>(requestMessage, Context.Logger, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<OrderItem> ListAllForOrganizationAsync(ListItemsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        return HelloAssoPager.PageAllAsync<ListItemsResponse, OrderItem>(
            (continuationToken, ct) =>
            {
                var pageRequest = request with { ContinuationToken = continuationToken };
                return ListForOrganizationAsync(pageRequest, tokens, ct);
            },
            cancellationToken);
    }
}
