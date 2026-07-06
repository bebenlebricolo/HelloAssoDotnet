namespace HelloAssoDotnet.Models.HelloAssoApi.Base;

/// <summary>
/// Pagination properties, as per returned by listing apis (such as the payment search)
/// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-payments"/>
/// </summary>
public record PaginationProperties
{
    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total element count
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page index
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// Total pages for this search
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Continuation token (used to retrieve next page)
    /// </summary>
    public string ContinuationToken { get; set; } = "";
}
