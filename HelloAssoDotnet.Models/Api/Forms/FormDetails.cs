namespace HelloAssoDotnet.Models.Api.Forms;

/// <summary>
/// Form details is an extension of the FormLightModel, with more information and data
/// </summary>
public record FormDetails : FormLightModel
{
    /// <summary>
    /// Organization logo, if any
    /// </summary>
    public string? OrganizationLogo { get; set; }

    /// <summary>
    /// Organization full name
    /// </summary>
    public string? OrganizationName { get; set; }

    /// <summary>
    /// List of tiers for this form
    /// </summary>
    public List<Tier>? Tiers {get;set;}

    /// <summary>
    /// Activity type of the event e.g. "Atelier(s) / Stage(s)" matching one of the
    /// provided type values <see href="https://dev.helloasso.com/reference/index#!/Values/Values_Get">provided here</see> or a custom value is allowed.
    /// </summary>
    public string? ActivityType { get; set; }

    /// <summary>
    /// Activity type identifier
    /// </summary>
    public int ActivityTypeId { get; set; }

    /// <summary>
    /// Where the form takes place (optional, used for events)
    /// </summary>
    public Place? Place { get; set; }

    /// <summary>
    /// The datetime (Inclusive) at which the users can start placing orders.
    /// If null the orders will be available as soon as the campaign is published.
    /// </summary>
    public DateTime? SaleStartDate { get; set; }

    /// <summary>
    /// The datetime (Inclusive) at which the sales end.
    /// If null the orders will be available until the end of the campaign.
    /// </summary>
    public DateTime? SaleEndDate { get; set; }

    /// <summary>
    /// Enum which represents the membership validity type
    /// </summary>
    public ValidityTypeEnum? ValidityType { get; set; }

    /// <summary>
    /// A message customized by the organization administrator.
    /// </summary>
    public string? PersonalizedMessage  { get; set; }
}
