using HelloAssoDotnet.Models.HelloAssoApi.Payment;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Filters for the organization/form payment search endpoints.
/// Leave a value uninitialized (blank/null) to let HelloAsso apply its default (no filter).
/// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-payments"/>
/// </summary>
public record SearchPaymentsRequest
{
    /// <summary>
    /// Free text used to search payments (typically the payer email, but also names).
    /// Maps to the <c>userSearchKey</c> query parameter.
    /// </summary>
    public string? UserSearchKey { get; set; }

    /// <summary>
    /// Restrict the results to these payment states. Leave empty to get everything.
    /// </summary>
    public List<PaymentState> States { get; set; } = new List<PaymentState>();

    /// <summary>
    /// Only include payments created after this date (inclusive).
    /// </summary>
    public DateTime? From { get; set; }

    /// <summary>
    /// Only include payments created before this date (inclusive).
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
