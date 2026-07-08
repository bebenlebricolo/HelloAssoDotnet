using HelloAssoDotnet.Models.Api.Base;
using HelloAssoDotnet.Models.Api.Forms;

namespace HelloAssoDotnet.Models.Api.Organizations;

/// <summary>
/// Public information about an organization (association).
/// Mirrors the API "OrganizationPublicModel" schema.
/// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug"/>
/// </summary>
public record OrganizationDetails
{
    /// <summary>
    /// Facebook page URL of the organization.
    /// </summary>
    public string? FacebookPage { get; set; }

    /// <summary>
    /// Images displayed in the organization public gallery.
    /// </summary>
    public List<ImageModel> GalleryImages { get; set; } = new List<ImageModel>();

    /// <summary>
    /// Long public description of the organization.
    /// </summary>
    public string? LongDescription { get; set; }

    /// <summary>
    /// Twitter page URL of the organization.
    /// </summary>
    public string? TwitterPage { get; set; }

    /// <summary>
    /// Videos displayed on the organization public page.
    /// </summary>
    public List<VideoModel> Videos { get; set; } = new List<VideoModel>();

    /// <summary>
    /// Website URL of the organization.
    /// </summary>
    public string? WebSite { get; set; }

    /// <summary>
    /// Whether the organization has been authenticated by HelloAsso.
    /// </summary>
    public bool? IsAuthenticated { get; set; }

    /// <summary>
    /// Whether the organization allows displaying its geographic coordinates.
    /// </summary>
    public bool? DisplayCoordinates { get; set; }

    /// <summary>
    /// Whether the organization is compliant for cash-in operations.
    /// </summary>
    public bool? IsCashInCompliant { get; set; }

    /// <summary>
    /// Banner image URL of the organization.
    /// </summary>
    public string? Banner { get; set; }

    /// <summary>
    /// Whether the organization is eligible to issue fiscal receipts.
    /// </summary>
    public bool FiscalReceiptEligibility { get; set; } = false;

    /// <summary>
    /// Whether the organization has enabled fiscal receipt issuance.
    /// </summary>
    public bool FiscalReceiptIssuanceEnabled { get; set; } = false;

    /// <summary>
    /// Legal type of the organization (association 1901, fondation, etc.).
    /// </summary>
    public OrganizationType Type { get; set; } = OrganizationType.Association1901;

    /// <summary>
    /// "Journal Officiel" category label, when available.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Postal address of the organization.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Geographic coordinates of the organization.
    /// </summary>
    public Geolocation? Geolocation { get; set; }

    /// <summary>
    /// Unique identifier assigned when creating the association (RNA number).
    /// </summary>
    public string? RnaNumber { get; set; }

    /// <summary>
    /// Logo URL of the organization.
    /// </summary>
    public string? Logo { get; set; }

    /// <summary>
    /// Organization display name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Role held by the caller on this organization.
    /// </summary>
    public GlobalRole Role { get; set; }

    /// <summary>
    /// City where the organization is registered.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Zip code of the organization.
    /// </summary>
    public string? ZipCode { get; set; }

    /// <summary>
    /// Short public description of the organization.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Last update date of the organization.
    /// </summary>
    public DateTime UpdateDate { get; set; }

    /// <summary>
    /// "Journal Officiel" category identifier, when available.
    /// </summary>
    public int? CategoryJoId { get; set; }

    /// <summary>
    /// Public URL of the organization page on HelloAsso.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Organization slug (lowercase, kebab-cased name used across the API).
    /// </summary>
    public string? OrganizationSlug { get; set; }
}
