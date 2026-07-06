using HelloAssoDotnet.Models.HelloAssoApi.Base;
using HelloAssoDotnet.Models.HelloAssoApi.Forms;

namespace HelloAssoDotnet.Models.HelloAssoApi.Payment;

/// <summary>
/// Represents a single pavement order from HelloAsso
/// </summary>
public record Order
{
    /// <summary>
    /// Order Id
    /// </summary>
    public int Id { get; set; } = 0;

    /// <summary>
    /// Checkout intent Id if available
    /// </summary>
    public int? CheckoutIntentId { get; set; }

    /// <summary>
    /// Order date
    /// </summary>
    public DateTime Date { get; set; } = DateTime.Now;

    /// <summary>
    /// Which form was used for this pavement (slug is the lowercase and kebab-case version of Form Name)
    /// </summary>
    public string FormSlug { get; set; } = string.Empty;

    /// <summary>
    /// Form plain name
    /// </summary>
    public string FormName { get; set; } = string.Empty;

    /// <summary>
    /// Form type
    /// </summary>
    public FormType FormType { get; set; } = FormType.Event;

    /// <summary>
    /// Organization plain name
    /// </summary>
    public string? OrganizationName { get; set; }

    /// <summary>
    /// Organization slugged name form
    /// </summary>
    public string? OrganizationSlug { get; set; }

    /// <summary>
    /// Organization type (usually, Association1901Rig)
    /// </summary>
    public string OrganizationType { get; set; } = string.Empty;

    /// <summary>
    /// Whether this organization is under Coluche Law or not
    /// </summary>
    public bool OrganizationIsUnderColucheLaw { get; set; } = false;

    /// <summary>
    /// Metadata
    /// </summary>
    public Dictionary<string, string> Meta  {get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Anonymous form ?
    /// </summary>
    public bool IsAnonymous { get; set; } = false;

    /// <summary>
    /// Anonymous hidden
    /// </summary>
    public bool IsAmountHidden { get; set; } = false;
}
