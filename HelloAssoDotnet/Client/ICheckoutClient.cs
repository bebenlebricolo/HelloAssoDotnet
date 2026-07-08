using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.Api.Checkout;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Read-only operations over HelloAsso Checkout. Creating a checkout intent is a write operation and is
/// intentionally out of scope for this read-only surface (planned for a later version).
/// </summary>
public interface ICheckoutClient
{
    /// <summary>
    /// Retrieves a checkout intent (and the associated order, once the payment is authorized).
    /// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-checkout-intents-checkoutintentid"/>
    /// </summary>
    /// <param name="checkoutIntentId">The checkout intent id.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The checkout intent.</returns>
    Task<Result<CheckoutIntentResponse>> GetIntentAsync(long checkoutIntentId, AuthTokens? tokens = null, CancellationToken cancellationToken = default);
}
