using HelloAssoDotnet.Models.HelloAssoApi.Base;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Common shape of every paginated HelloAsso listing response: a <see cref="Data"/> page and
/// the <see cref="Pagination"/> metadata used to iterate further (via the continuation token).
/// This allows a single generic auto-pager to walk any listing endpoint.
/// </summary>
/// <typeparam name="T">Type of the elements contained in a page.</typeparam>
public interface IPaginatedResponse<out T>
{
    /// <summary>
    /// Current page of results.
    /// </summary>
    IReadOnlyList<T> Data { get; }

    /// <summary>
    /// Pagination metadata (page size/index, totals and continuation token).
    /// </summary>
    PaginationProperties Pagination { get; }
}
