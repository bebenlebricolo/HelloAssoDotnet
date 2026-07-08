namespace HelloAssoDotnet.Models.Api.Values;

/// <summary>
/// A company legal status entry. Mirrors the API "CompanyLegalStatusModel" schema.
/// <see aref="https://dev.helloasso.com/reference/get_values-company-legal-status"/>
/// </summary>
public record CompanyLegalStatus
{
    /// <summary>
    /// Stable identifier of the legal status.
    /// </summary>
    public int Id { get; set; } = 0;

    /// <summary>
    /// Human readable label.
    /// </summary>
    public string? Label { get; set; }
}

/// <summary>
/// An organization category ("Journal Officiel" category).
/// Mirrors the API "OrganismCategoryModel" schema.
/// <see aref="https://dev.helloasso.com/reference/get_values-organization-categories"/>
/// </summary>
public record OrganizationCategory
{
    /// <summary>
    /// Stable identifier of the category.
    /// </summary>
    public int Id { get; set; } = 0;

    /// <summary>
    /// Human readable label.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Shortened human readable label.
    /// </summary>
    public string? ShortLabel { get; set; }
}

/// <summary>
/// A public tag usable to categorize forms/organizations in the directory.
/// Mirrors the API "PublicTagModel" schema.
/// <see aref="https://dev.helloasso.com/reference/get_values-tags"/>
/// </summary>
public record PublicTag
{
    /// <summary>
    /// Tag name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Relevance score of the tag.
    /// </summary>
    public double Score { get; set; } = 0;
}

/// <summary>
/// A form sub-type (activity type) available for a given form type.
/// Mirrors the API "FormActivityModel" schema.
/// <see aref="https://dev.helloasso.com/reference/get_values-form-formtype-types"/>
/// </summary>
public record FormSubType
{
    /// <summary>
    /// Stable identifier of the sub-type.
    /// </summary>
    public int Id { get; set; } = 0;

    /// <summary>
    /// Human readable label.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Shortened human readable label.
    /// </summary>
    public string? ShortLabel { get; set; }
}
