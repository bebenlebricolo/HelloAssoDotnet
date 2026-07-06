using HelloAssoDotnet.Models.HelloAssoApi.Base;
using HelloAssoDotnet.Models.HelloAssoApi.Forms;
using HelloAssoDotnet.Models.HelloAssoApi.Payment;

namespace HelloAssoDotnet.Models.HelloAssoApi.Order;

/// <summary>
/// Retrieved from https://api.helloasso.com/v5/orders/{orderId} endpoint$
/// <see aref="https://dev.helloasso.com/reference/get_orders-orderid"/>
/// </summary>
public record OrderDetails
{
    /// <summary>
    /// Who paid this Order (might be different to the actual receiver of this Order)
    /// </summary>
    public Payer Payer { get; set; } = new Payer();

    /// <summary>
    /// All items of the order
    /// </summary>
    public List<OrderItem>? Items { get; set; } = null;

    /// <summary>
    /// All payments of the order
    /// </summary>
    public List<OrderPayment>? Payments { get; set; } = null;

    /// <summary>
    /// Represents the financial details associated with an order, including total amount, VAT, and discount.
    /// </summary>
    public record Amount
    {
        /// <summary>
        /// Total amount in cents
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Vat amount in cents
        /// </summary>
        public int Vat { get; set; }

        /// <summary>
        /// Discount amount in cents
        /// </summary>
        public int Discount { get; set; }
    }

    /// <summary>
    /// The ID of the order
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Order creation date
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// FormSlug (lowercase name of the form without special characters)
    /// </summary>
    public string? FormSlug { get; set; } = null;

    /// <summary>
    /// The type of the form
    /// </summary>
    public FormType FormType { get; set; } = FormType.Event;

    /// <summary>
    /// The organization name.
    /// </summary>
    public string? OrganizationName { get; set; } = null;

    /// <summary>
    /// OrganizationSlug (lowercase name of the organization without special characters)
    /// </summary>
    public string? OrganizationSlug { get; set; } = null;

    /// <summary>
    /// Organization type
    /// </summary>
    public OrganizationType OrganizationType { get; set; } = OrganizationType.Association1901;

    /// <summary>
    /// Whether the organization is subject to the coluche law or not
    /// </summary>
    public bool OrganizationIsUnderColucheLaw { get; set; } = false;

    /// <summary>
    /// Checkout intent Id if available
    /// </summary>
    public int? CheckoutIntentId { get; set; } = null;

    /// <summary>
    /// Metadata for this Order object.
    /// </summary>
    public Dictionary<string,string> Meta { get; set; } = new Dictionary<string, string>();
}
