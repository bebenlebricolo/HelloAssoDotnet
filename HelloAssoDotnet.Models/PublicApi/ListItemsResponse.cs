using HelloAssoDotnet.Models.HelloAssoApi.Base;
using HelloAssoDotnet.Models.HelloAssoApi.Order;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Paginated list of items (sold entries) returned by organization/form item listings.
/// </summary>
public record ListItemsResponse : IPaginatedResponse<OrderItem>
{
    /// <summary>
    /// Retrieved items for the current page.
    /// </summary>
    public List<OrderItem> Data { get; set; } = new List<OrderItem>();

    /// <summary>
    /// Pagination properties.
    /// </summary>
    public PaginationProperties Pagination { get; set; } = new PaginationProperties();

    /// <inheritdoc />
    IReadOnlyList<OrderItem> IPaginatedResponse<OrderItem>.Data => Data;
}
