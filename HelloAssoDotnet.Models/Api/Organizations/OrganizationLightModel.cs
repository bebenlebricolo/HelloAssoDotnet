using HelloAssoDotnet.Models.Api.Base;

namespace HelloAssoDotnet.Models.Api.Organizations;

/// <summary>
/// Lightweight organization descriptor, as returned by directory and partner/user listings.
/// Mirrors the API "OrganizationLightModel" schema.
/// </summary>
public record OrganizationLightModel
{
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
    /// Organization description.
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
