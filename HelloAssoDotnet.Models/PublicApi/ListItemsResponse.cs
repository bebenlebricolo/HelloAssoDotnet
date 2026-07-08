using HelloAssoDotnet.Models.Api.Order;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Paginated list of items (sold entries) returned by organization/form item listings.
/// </summary>
public record ListItemsResponse : PaginatedResponse<OrderItem>;
