using HelloAssoDotnet.Models.HelloAssoApi.Base;
using HelloAssoDotnet.Models.HelloAssoApi.Order;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Paginated list of orders returned by organization/form order listings.
/// </summary>
public record ListOrdersResponse : IPaginatedResponse<OrderDetails>
{
    /// <summary>
    /// Retrieved orders for the current page.
    /// </summary>
    public List<OrderDetails> Data { get; set; } = new List<OrderDetails>();

    /// <summary>
    /// Pagination properties.
    /// </summary>
    public PaginationProperties Pagination { get; set; } = new PaginationProperties();

    /// <inheritdoc />
    IReadOnlyList<OrderDetails> IPaginatedResponse<OrderDetails>.Data => Data;
}
