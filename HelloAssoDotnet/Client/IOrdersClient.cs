using HelloAssoDotnet.Models.HelloAssoApi.Auth;
using HelloAssoDotnet.Models.HelloAssoApi.Order;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Read-only operations over orders (global by id, and scoped to the configured organization).
/// </summary>
public interface IOrdersClient
{
    /// <summary>
    /// Retrieves the details of an order by its id.
    /// <see aref="https://dev.helloasso.com/reference/get_orders-orderid"/>
    /// </summary>
    /// <param name="orderId">The unique identifier of the order.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The order details.</returns>
    Task<Result<OrderDetails>> GetAsync(int orderId, AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the orders placed on the configured organization.
    /// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-orders"/>
    /// </summary>
    /// <param name="request">Filtering and pagination parameters.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A single page of orders.</returns>
    Task<Result<ListOrdersResponse>> ListForOrganizationAsync(ListOrdersRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enumerates every order of the configured organization, transparently following the continuation token.
    /// </summary>
    /// <param name="request">Filtering parameters (pagination is handled automatically).</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An async stream of orders.</returns>
    IAsyncEnumerable<OrderDetails> ListAllForOrganizationAsync(ListOrdersRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the Event ticket's PDF(s) from a payment response (that refers to an event booking ticket).
    /// </summary>
    /// <param name="payment">Sample payment data (as per retrieved with the payments sub-client).</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>One PDF stream per ticket of the order.</returns>
    Task<Result<List<Stream>>> GetEventTicketsPdfAsync(PaymentResponse payment, AuthTokens? tokens = null, CancellationToken cancellationToken = default);
}
