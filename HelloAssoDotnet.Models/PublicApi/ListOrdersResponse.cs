using HelloAssoDotnet.Models.Api.Order;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Paginated list of orders returned by organization/form order listings.
/// </summary>
public record ListOrdersResponse : PaginatedResponse<OrderDetails>;
