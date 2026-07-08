using HelloAssoDotnet.Models.Api.Order;
using HelloAssoDotnet.Models.Api.Payment;

namespace HelloAssoDotnet.Models.Api.Forms;

/// <summary>
/// Aggregated statistics for a form. Mirrors the API "FormStatsModel" schema.
/// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-forms-formtype-formslug-stats"/>
/// </summary>
public record FormStats
{
    /// <summary>
    /// Total number of participants on the form.
    /// </summary>
    public int TotalParticipant { get; set; } = 0;

    /// <summary>
    /// Per-tier statistics for the tiers that are not grouped.
    /// </summary>
    public List<TierStats> UnGroupedTiers { get; set; } = new List<TierStats>();

    /// <summary>
    /// Per-tier statistics for the additional options (extra tiers).
    /// </summary>
    public List<TierStats> AdditionalOptions { get; set; } = new List<TierStats>();
}

/// <summary>
/// Statistics for a single tier of a form. Mirrors the API "TierStatsModel" schema.
/// </summary>
public record TierStats
{
    /// <summary>
    /// Stable identifier of the tier.
    /// </summary>
    public int Id { get; set; } = 0;

    /// <summary>
    /// Tier label.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Tier description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Number of entries already taken for this tier.
    /// </summary>
    public int EntriesTaken { get; set; } = 0;

    /// <summary>
    /// Maximum number of entries available for this tier, when limited.
    /// </summary>
    public int? MaxEntries { get; set; }

    /// <summary>
    /// Price of the tier, in cents.
    /// </summary>
    public int? Price { get; set; }

    /// <summary>
    /// Minimum amount accepted for the tier, in cents (for "pay what you want" tiers).
    /// </summary>
    public int? MinAmount { get; set; }

    /// <summary>
    /// Pricing category of the tier (fixed, pay-what-you-want, free).
    /// </summary>
    public PriceCategory PriceCategory { get; set; }

    /// <summary>
    /// Functional type of the tier.
    /// </summary>
    public TierType TierType { get; set; }

    /// <summary>
    /// Whether the tier is currently enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = false;
}
