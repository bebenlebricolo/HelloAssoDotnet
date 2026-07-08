namespace HelloAssoDotnet.Models.Api.Forms;

/// <summary>
/// Lighter version of a Form model (the full one being <see cref="FormDetails"/>)
/// </summary>
public record FormLightModel
{
    /// <summary>
    /// Banner of the said form
    /// </summary>
    public Logo? Banner { get; set; }

    /// <summary>
    /// Original currency used for this form
    /// </summary>
    public CurrencyEnum? Currency {get;set;} = CurrencyEnum.Euro;

    /// <summary>
    /// Form description
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// Optional metadata for this record
    /// </summary>
    public Dictionary<string,string> Meta {get;set;} = new Dictionary<string, string>();

    /// <summary>
    /// Whether this form is visible or not
    /// </summary>
    public PublicationState State {get;set;} = PublicationState.Private;

    /// <summary>
    /// The datetime of the activity start
    /// </summary>
    public DateTime? StartDate {get;set;} = DateTime.Now;

    /// <summary>
    /// The datetime of the activity end
    /// </summary>
    public DateTime? EndDate {get;set;} = DateTime.Now;

    /// <summary>
    /// Form's logo (changed from a raw string to an actual data structure)
    /// </summary>
    public Logo? Logo {get;set;}

    /// <summary>
    /// Form's title (optional ?)
    /// </summary>
    public string? Title { get; set; } = "";

    /// <summary>
    /// Form's private title (optional ?)
    /// </summary>
    public string? PrivateTitle { get; set; } = "";

    /// <summary>
    /// Url of the widget button
    /// </summary>
    public string? WidgetButtonUrl { get; set; } = "";

    /// <summary>
    /// Url of the form widget
    /// </summary>
    public string? WidgetFullUrl { get; set; } = "";

    /// <summary>
    /// Url of the horizontal vignette widget
    /// </summary>
    public string? WidgetVignetteHorizontalUrl  { get; set; } = "";

    /// <summary>
    /// Url of the vertical vignette widget
    /// </summary>
    public string? WidgetVignetteVerticalUrl { get; set; } = "";

    /// <summary>
    /// Url of the counter widget
    /// </summary>
    public string? WidgetCounterUrl  { get; set; } = "";

    /// <summary>
    /// Form's slug (should be filled in, that's the key of queries)
    /// </summary>
    public string? FormSlug { get; set; } = "";

    /// <summary>
    /// Form type
    /// </summary>
    public FormType? FormType { get; set; }

    /// <summary>
    /// Form's Url
    /// </summary>
    public string? Url { get; set; } = "";

    /// <summary>
    /// The organization slug
    /// </summary>
    public string? OrganizationSlug { get; set; } = "";
}
