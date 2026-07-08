using HelloAssoDotnet.Models.Api.Forms;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Filters for the organization/form order listing endpoints.
/// Leave a value uninitialized (blank/null) to let HelloAsso apply its default (no filter).
/// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-orders"/>
/// </summary>
public record ListOrdersRequest
{
    /// <summary>
    /// Restrict the results to these form types. Leave empty to get everything.
    /// </summary>
    public List<FormType> FormTypes { get; set; } = new List<FormType>();

    /// <summary>
    /// Only include orders created after this date (inclusive).
    /// </summary>
    public DateTime? From { get; set; }

    /// <summary>
    /// Only include orders created before this date (inclusive).
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
