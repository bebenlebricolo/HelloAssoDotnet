using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.Api.Order;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Read-only operations over items (individual entries sold within orders).
/// </summary>
public interface IItemsClient
{
    /// <summary>
    /// Retrieves the detail of a single item contained in an order.
    /// <see aref="https://dev.helloasso.com/reference/get_items-itemid"/>
    /// </summary>
    /// <param name="itemId">The item id.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The item detail.</returns>
    Task<Result<OrderItem>> GetAsync(int itemId, AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the items sold by the configured organization.
    /// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-items"/>
    /// </summary>
    /// <param name="request">Filtering and pagination parameters.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A single page of items.</returns>
    Task<Result<ListItemsResponse>> ListForOrganizationAsync(ListItemsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enumerates every item of the configured organization, transparently following the continuation token.
    /// </summary>
    /// <param name="request">Filtering parameters (pagination is handled automatically).</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An async stream of items.</returns>
    IAsyncEnumerable<OrderItem> ListAllForOrganizationAsync(ListItemsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default);
}
