using HelloAssoDotnet.Models.HelloAssoApi.Payment;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Filters for the organization/form item (sold entries) listing endpoints.
/// Leave a value uninitialized (blank/null) to let HelloAsso apply its default (no filter).
/// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-items"/>
/// </summary>
public record ListItemsRequest
{
    /// <summary>
    /// Restrict the results to these item states. Leave empty to get everything.
    /// </summary>
    public List<ItemState> States { get; set; } = new List<ItemState>();

    /// <summary>
    /// Only include items created after this date (inclusive).
    /// </summary>
    public DateTime? From { get; set; }

    /// <summary>
    /// Only include items created before this date (inclusive).
    /// </summary>
    public DateTime? To { get; set; }

    /// <summary>
    /// Queried page index (1-based).
    /// </summary>
    public int? PageIndex { get; set; }

    /// <summary>
    /// Queried page size.
    /// </summary>
    public int? PageSize { get; set; }

    /// <summary>
    /// Continuation token used to iterate pages.
    /// </summary>
    public string? ContinuationToken { get; set; }
}
