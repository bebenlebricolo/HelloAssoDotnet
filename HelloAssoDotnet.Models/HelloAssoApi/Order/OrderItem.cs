using HelloAssoDotnet.Models.HelloAssoApi.Payment;

namespace HelloAssoDotnet.Models.HelloAssoApi.Order;

/// <summary>
/// Represents custom fields for an order item
/// </summary>
public record CustomFields
{
    /// <summary>
    /// Gets or sets the unique identifier for the custom field
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the custom field
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Defines the type of custom field
    /// </summary>
    public enum Type
    {
        /// <summary>
        /// Date input field type
        /// </summary>
        Date,

        /// <summary>
        /// Single-line text input field type
        /// </summary>
        TextInput,

        /// <summary>
        /// Multi-line text input field type
        /// </summary>
        FreeText,

        /// <summary>
        /// Selection list field type
        /// </summary>
        ChoiceList,

        /// <summary>
        /// File upload field type
        /// </summary>
        File,

        /// <summary>
        /// Boolean yes/no field type
        /// </summary>
        YesNo,

        /// <summary>
        /// Phone number field type
        /// </summary>
        Phone,

        /// <summary>
        /// Postal code field type
        /// </summary>
        Zipcode,

        /// <summary>
        /// Numeric field type
        /// </summary>
        Number
    }

    /// <summary>
    /// Participant or user answer
    /// </summary>
    public string? Answer { get; set; }
}

/// <summary>
/// Represents additional options available for an order item
/// </summary>
public record Options
{
    /// <summary>
    /// Name of the option
    /// </summary>
    public string? Name { get; set; } = null;

    /// <summary>
    /// Amount of the option in cents
    /// </summary>
    public int Amount { get; set; } = 0;

    /// <summary>
    /// Price category
    /// </summary>
    public PriceCategory PriceCategory { get; set; }

    /// <summary>
    /// Option is required or optional
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// Custom fields related to this option
    /// </summary>
    public List<CustomFields>? CustomFields { get; set; } = null;
}

/// <summary>
/// Represents a payment share associated with an order item
/// </summary>
public record SharePayments
{
    /// <summary>
    /// ID of the payment
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Amount of the item paid on this payment term (in cents)
    /// </summary>
    public int ShareAmount { get; set; }
}

/// <summary>
/// Represents an item in an order with its associated details and payment information
/// </summary>
public record OrderItem
{
    /// <summary>
    /// Payments linked to this item and each share between the item and the payment
    /// </summary>
    public List<SharePayments>? Payments { get; set; }

    /// <summary>
    /// Optional name of this order
    /// </summary>
    public string? Name { get; set; } = null;

    /// <summary>
    /// Receiving User (the user that'll benefit from the tickets)
    /// </summary>
    public record User
    {
        /// <summary>
        /// User first name
        /// </summary>
        public string? FirstName { get; set; } = null;

        /// <summary>
        /// User last name
        /// </summary>
        public string? LastName { get; set; } = null;
    }

    /// <summary>
    /// Order pricing category
    /// </summary>
    public PriceCategory PriceCategory { get; set; } = PriceCategory.Fixed;

    /// <summary>
    /// Minimum amount that was specified on the tier (in cents)
    /// </summary>
    public int? MinAmount { get; set; } = null;

    /// <summary>
    /// Represents a custom discount applied on the item
    /// </summary>
    public record Discount
    {
        /// <summary>
        /// The discount code applied on the item
        /// </summary>
        public string? Code { get; set; } = null;

        /// <summary>
        /// The discount amount in cents
        /// </summary>
        public int Amount { get; set; } = 0;
    }

    /// <summary>
    /// Custom fields related to this item
    /// </summary>
    public List<CustomFields>? CustomFields { get; set; } = null;

    /// <summary>
    /// Extra options taken with this item
    /// </summary>
    public List<Options>? Options { get; set; } = null;

    /// <summary>
    /// The Ticket Url
    /// </summary>
    public string? TicketUrl { get; set; } = null;

    /// <summary>
    /// The item QrCode (for ticket scanning only)
    /// </summary>
    public string? QrCode { get; set; } = null;

    /// <summary>
    /// The Membership Card Url
    /// </summary>
    /// <returns></returns>
    public string? MembershipCardUrl { get; set; } = null;

    /// <summary>
    /// The day of levy for monthly donation only
    /// </summary>
    public int? DayOfLevy { get; set; } = null;

    /// <summary>
    /// Tier description
    /// </summary>
    /// <returns></returns>
    public string? TierDescription { get; set; } = null;

    /// <summary>
    /// Tier id
    /// </summary>
    public int? TierId { get; set; } = null;

    /// <summary>
    /// Order's comment
    /// </summary>
    public string? Comment { get; set; } = null;

    /// <summary>
    /// ID of the Item
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Total item Price in cents (after discount without extra options)
    /// </summary>
    public int Amount { get; set; } = 0;

    /// <summary>
    /// Kind of payment for this Order
    /// </summary>
    public TierType Type { get; set; } = TierType.Payment;

    /// <summary>
    /// The raw amount (without reduction)
    /// </summary>
    public int? InitialAmount { get; set; } = null;

    /// <summary>
    /// Order payment processing state
    /// </summary>
    public ItemState State { get; set; } = ItemState.Processed;


}
