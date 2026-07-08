using HelloAssoDotnet.Models.HelloAssoApi.Order;

namespace HelloAssoDotnet.Models.HelloAssoApi.Checkout;

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
    public long Id { get; set; }

    /// <summary>
    /// URL the payer must be redirected to in order to complete the checkout.
    /// </summary>
    public string? RedirectUrl { get; set; }

    /// <summary>
    /// The resulting order, only available once the payment is authorized.
    /// </summary>
    public OrderDetails? Order { get; set; }

    /// <summary>
    /// Free-form metadata that was attached when the intent was created.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
}
