using System.Text.Json;
using HelloAssoDotnet.Models.Api.Order;

namespace HelloAssoDotnet.Models.Api.Checkout;

/// <summary>
/// A checkout intent retrieved from HelloAsso. The associated <see cref="Order"/> is only present once the
/// payment has been authorized.
/// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-checkout-intents-checkoutintentid"/>
/// </summary>
public record CheckoutIntentResponse
{
    /// <summary>
    /// Unique identifier of the checkout intent.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// URL the payer must be redirected to in order to complete the checkout.
    /// </summary>
    public string? RedirectUrl { get; set; }

    /// <summary>
    /// The resulting order, only available once the payment is authorized.
    /// </summary>
    public OrderDetails? Order { get; set; }

    /// <summary>
    /// Free-form metadata (arbitrary JSON object) that was attached when the intent was created.
    /// Only present if metadata were sent on the checkout form initialization. Kept as a raw
    /// <see cref="JsonElement"/> because the shape is decided by the integrator, not by HelloAsso.
    /// </summary>
    public JsonElement? Metadata { get; set; }
}
