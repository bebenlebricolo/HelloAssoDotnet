using HelloAssoDotnet.Models.Configuration;
using HelloAssoDotnet.Models.HelloAssoApi.Auth;
using HelloAssoDotnet.Models.HelloAssoApi.Forms;
using HelloAssoDotnet.Models.HelloAssoApi.Order;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Defines the operations for interacting with the HelloAsso system.
/// Provides methods for authentication, retrieving forms, payments, orders, and related details.
/// </summary>
public interface IHelloAssoClient
{
    /// <summary>
    /// Authenticate webserver application.
    /// Tokens are cached and retrieved from there if we can, otherwise token is renewed and stored again.
    /// </summary>
    public Task<Result<AuthTokens>> AuthenticateAsync();

    /// <summary>
    /// Refreshes the given Jwt token
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns></returns>
    public Task<Result<AuthTokens>> RefreshTokenAsync(AuthTokens tokens);

    /// <summary>
    /// Retrieves all payments for a given user.
    /// Returns null, if none can be found.
    /// </summary>
    /// <param name="email">user email (used as a search parameter)</param>
    /// <param name="tokens">Authentication tokens that'll be used for this request</param>
    /// <returns></returns>
    public Task<Result<SearchPaymentResponse>> GetPaymentForUserAsync(string email, AuthTokens tokens);

    /// <summary>
    /// Retrieves a list of all forms for an association using provided request model.
    /// </summary>
    /// <param name="requestModel">Request model for this HelloAsso api</param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    public Task<Result<ListOrganizationFormsResponse>> GetFormsFromOrganization(ListOrganizationFormsRequest requestModel, AuthTokens tokens);

    /// <summary>
    /// Retrieves a single form details.
    /// </summary>
    /// <param name="formSlug">exact form slug</param>
    /// <param name="formType">Required form type</param>
    /// <param name="tokens">Authentication token for HelloAsso Apis</param>
    /// <returns></returns>
    public Task<Result<FormDetails>> GetFormDetailsAsync(string formSlug, FormType formType, AuthTokens tokens);

    /// <summary>
    /// Retrieves a single Payment for a user, given the paymentId.
    /// </summary>
    /// <param name="paymentId">Payment id</param>
    /// <param name="tokens">Authentication tokens used with this request</param>
    /// <param name="withFailedRefundOperations">Will also retrieve failed payments</param>
    /// <returns>Payment response, if an item was found; null otherwise</returns>
    public Task<Result<PaymentResponse>> GetPaymentDetailsAsync(int paymentId, AuthTokens tokens, bool withFailedRefundOperations = false);

    /// <summary>
    /// Retrieves the Receipt's PDF.
    /// Note that this only works when tokens have the full roles assigned (conventional Jwt tokens retrieved from API key DO NOT WORK!)
    /// </summary>
    /// <param name="payment">Sample payment data (as per retrieved with <see cref="GetPaymentDetailsAsync"/> or <see cref="GetPaymentForUserAsync"/></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    public Task<Result<Stream>> GetPaymentReceiptPdfAsync(PaymentResponse payment, AuthTokens tokens);


    /// <summary>
    /// Retrieves the Event ticket's PDF(s) from a payment response (that refers to an event booking ticket)
    /// </summary>
    /// <param name="payment">Sample payment data (as per retrieved with <see cref="GetPaymentDetailsAsync"/> or <see cref="GetPaymentForUserAsync"/></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    public Task<Result<List<Stream>>> GetEventTicketPdf(PaymentResponse payment, AuthTokens tokens);

    /// <summary>
    ///
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    public Task<Result<OrderDetails>> GetOrderDetailsAsync(int orderId, AuthTokens tokens);

    /// <summary>
    /// Sets the client configuration (used by calling layers)
    /// </summary>
    public ClientConfig Config { get; set; }
}
