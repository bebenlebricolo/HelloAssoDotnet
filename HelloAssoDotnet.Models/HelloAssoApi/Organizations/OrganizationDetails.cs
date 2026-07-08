using HelloAssoDotnet.Models.HelloAssoApi.Base;
using HelloAssoDotnet.Models.HelloAssoApi.Forms;

namespace HelloAssoDotnet.Models.HelloAssoApi.Organizations;

/// <summary>
/// Public information about an organization (association).
/// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug"/>
/// </summary>
public record OrganizationDetails
{
    /// <summary>
    /// Organization display name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Organization slug (lowercase, kebab-cased name used across the API).
    /// </summary>
    public string? OrganizationSlug { get; set; }

    /// <summary>
    /// Short public description of the organization.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Legal type of the organization (association 1901, fondation, etc.).
    /// </summary>
    public OrganizationType Type { get; set; } = OrganizationType.Association1901;

    /// <summary>
    /// "Journal Officiel" category label, when available.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Logo of the organization.
    /// </summary>
    public Logo? Logo { get; set; }

    /// <summary>
    /// Public URL of the organization page on HelloAsso.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// City where the organization is registered.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Zip code of the organization.
    /// </summary>
    public string? ZipCode { get; set; }

    /// <summary>
    /// Whether the organization is subject to the "loi Coluche".
    /// </summary>
    public bool IsUnderColucheLaw { get; set; }

    /// <summary>
    /// Original currency used by the organization.
    /// </summary>
    public CurrencyEnum? Currency { get; set; } = CurrencyEnum.Euro;

    /// <summary>
    /// Additional metadata.
    /// </summary>
    public Dictionary<string, string> Meta { get; set; } = new Dictionary<string, string>();
}
