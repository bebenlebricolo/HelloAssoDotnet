namespace HelloAssoDotnet.Models.HelloAssoApi.Values;

/// <summary>
/// A company legal status entry.
/// <see aref="https://dev.helloasso.com/reference/get_values-company-legal-status"/>
/// </summary>
public record CompanyLegalStatus
{
    /// <summary>
    /// Stable identifier of the legal status.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Machine value/key of the legal status.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Human readable label.
    /// </summary>
    public string? Label { get; set; }
}

/// <summary>
/// An organization category ("Journal Officiel" category).
/// <see aref="https://dev.helloasso.com/reference/get_values-organization-categories"/>
/// </summary>
public record OrganizationCategory
{
    /// <summary>
    /// Stable identifier of the category.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Machine value/key of the category.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Human readable label.
    /// </summary>
    public string? Label { get; set; }
}

/// <summary>
/// A public tag usable to categorize forms/organizations in the directory.
/// <see aref="https://dev.helloasso.com/reference/get_values-tags"/>
/// </summary>
public record PublicTag
{
    /// <summary>
    /// Tag name.
    /// </summary>
    public string? Name { get; set; }
}

/// <summary>
/// A form sub-type (activity type) available for a given form type.
/// <see aref="https://dev.helloasso.com/reference/get_values-form-formtype-types"/>
/// </summary>
public record FormSubType
{
    /// <summary>
    /// Stable identifier of the sub-type.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Human readable label.
    /// </summary>
    public string? Label { get; set; }
}
