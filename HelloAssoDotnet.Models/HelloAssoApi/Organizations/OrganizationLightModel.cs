using HelloAssoDotnet.Models.HelloAssoApi.Base;

namespace HelloAssoDotnet.Models.HelloAssoApi.Organizations;

/// <summary>
/// Lightweight organization descriptor, as returned by directory and partner/user listings.
/// </summary>
public record OrganizationLightModel
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
    /// Legal type of the organization.
    /// </summary>
    public OrganizationType Type { get; set; } = OrganizationType.Association1901;

    /// <summary>
    /// City where the organization is registered.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Zip code of the organization.
    /// </summary>
    public string? ZipCode { get; set; }

    /// <summary>
    /// Public URL of the organization page on HelloAsso.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    public Dictionary<string, string> Meta { get; set; } = new Dictionary<string, string>();
}
