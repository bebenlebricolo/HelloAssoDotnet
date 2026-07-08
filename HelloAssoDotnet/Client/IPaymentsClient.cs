using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Read-only operations over payments (search on the configured organization, and global by id).
/// </summary>
public interface IPaymentsClient
{
    /// <summary>
    /// Searches payments on the configured organization using the given filters.
    /// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-payments"/>
    /// </summary>
    /// <param name="request">Filtering and pagination parameters.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A single page of payments.</returns>
    Task<Result<SearchPaymentResponse>> SearchAsync(SearchPaymentsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enumerates every payment matching the filters, transparently following the continuation token.
    /// </summary>
    /// <param name="request">Filtering parameters (pagination is handled automatically).</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An async stream of payments.</returns>
    IAsyncEnumerable<PaymentResponse> SearchAllAsync(SearchPaymentsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all authorized payments for a given user (convenience over <see cref="SearchAsync"/>).
    /// </summary>
    /// <param name="email">User email (used as a search parameter).</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The authorized payments matching the email.</returns>
    Task<Result<SearchPaymentResponse>> SearchForUserAsync(string email, AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single payment by id.
    /// <see aref="https://dev.helloasso.com/reference/get_payments-paymentid"/>
    /// </summary>
    /// <param name="paymentId">Payment id.</param>
    /// <param name="withFailedRefundOperations">Also retrieve failed refund operations.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The payment details.</returns>
    Task<Result<PaymentResponse>> GetAsync(int paymentId, bool withFailedRefundOperations = false, AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the Receipt's PDF.
    /// Note that this only works when tokens have the full roles assigned (conventional Jwt tokens retrieved from API key DO NOT WORK!)
    /// </summary>
    /// <param name="payment">Sample payment data (as per retrieved with the payments sub-client).</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The receipt PDF stream.</returns>
    Task<Result<Stream>> GetReceiptPdfAsync(PaymentResponse payment, AuthTokens? tokens = null, CancellationToken cancellationToken = default);
}
