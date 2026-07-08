using HelloAssoDotnet.Models.Api.Base;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Common shape of every paginated HelloAsso listing response: a <see cref="Data"/> page and the
/// <see cref="Pagination"/> metadata used to iterate further (via the continuation token). Listing endpoints
/// use this record directly (there is nothing endpoint-specific to add), and the generic auto-pager walks any
/// instance of it.
/// </summary>
/// <typeparam name="T">Type of the elements contained in a page.</typeparam>
public record PaginatedResponse<T>
{
    /// <summary>
    /// Current page of results.
    /// </summary>
    public List<T> Data { get; set; } = new List<T>();

    /// <summary>
    /// Pagination metadata (page size/index, totals and continuation token).
    /// </summary>
    public PaginationProperties Pagination { get; set; } = new PaginationProperties();
}
