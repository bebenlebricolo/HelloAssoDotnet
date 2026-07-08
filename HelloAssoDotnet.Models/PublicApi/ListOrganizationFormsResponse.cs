using HelloAssoDotnet.Models.Api.Forms;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Paginated list of forms returned by the organization forms listing.
/// </summary>
public record ListOrganizationFormsResponse : PaginatedResponse<FormLightModel>;
