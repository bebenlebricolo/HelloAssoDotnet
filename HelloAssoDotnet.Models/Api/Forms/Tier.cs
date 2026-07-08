using HelloAssoDotnet.Models.Api.Payment;

namespace HelloAssoDotnet.Models.Api.Forms;

/// <summary>
/// Represents a form's tier datamodel, reproduced from https://dev.helloasso.com/reference/get_organizations-organizationslug-forms-formtype-formslug-public
/// </summary>
public record Tier
{
    /// <summary>
    /// Tier's Id
    /// </summary>
    public uint Id { get; set; }

    /// <summary>
    /// Optional text label
    /// </summary>
    public string? Label  { get; set; }

    /// <summary>
    /// Tier's description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Tier's type
    /// </summary>
    public TierType? TierType { get; set; }

    /// <summary>
    /// the Price in cents
    /// if price equals 0 then it is free or there is a MinAmount
    /// </summary>
    public int? Price { get; set; }

    /// <summary>
    /// Vat rate if applicable
    /// Amount have to be 0.10 for 10%
    /// </summary>
    public double VatRate { get; set; }

    /// <summary>
    /// If set, it means the payment is free to choose,
    /// according to the specified minAmount in cents
    /// </summary>
    public int? MinAmount { get; set; }

    /// <summary>
    /// Payment frequency
    /// </summary>
    public PaymentFrequencyEnum PaymentFrequency  { get; set; } = PaymentFrequencyEnum.Single;

    /// <summary>
    /// Max quantity buyable in this cart
    /// </summary>
    public int? MaxPerUser { get; set; }

    /// <summary>
    /// Metadata model
    /// </summary>
    public Dictionary<string,string>? Meta { get; set; }

    /// <summary>
    /// The datetime (Inclusive) at which the users can start buying this tier.
    /// If null the tier will be available at the start of the event.
    /// </summary>
    public DateTime? SaleStartDate { get; set; }

    /// <summary>
    /// The datetime (Inclusive) at which the tier is no longer available.
    /// If null the tier will be available until the end of the event.
    /// </summary>
    public DateTime? SaleEndDate { get; set; }

    /// <summary>
    /// Whether this is eligible to a deduction
    /// </summary>
    public bool IsEligibleTaxReceipt { get; set; }

    /// <summary>
    /// Terms of tier
    /// </summary>
    public List<Term>? Terms { get; set; }

    /// <summary>
    /// Tier's picture
    /// </summary>
    public Picture? Picture { get; set; }

    /// <summary>
    /// True means this tier must be paid in the initial payment, false means it can be paid in payment with installments
    /// Null when the form payment terms are disabled or not compatible with the related form
    /// </summary>
    public bool? IsExcludedFromFormPaymentTerms { get; set; }
}
