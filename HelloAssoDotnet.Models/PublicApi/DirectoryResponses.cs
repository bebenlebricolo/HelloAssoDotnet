using HelloAssoDotnet.Models.Api.Forms;
using HelloAssoDotnet.Models.Api.Organizations;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Paginated list of forms returned by the public directory forms search.
/// </summary>
public record DirectoryFormsResponse : PaginatedResponse<FormLightModel>;

/// <summary>
/// Paginated list of organizations returned by the public directory organizations search.
/// </summary>
public record DirectoryOrganizationsResponse : PaginatedResponse<OrganizationLightModel>;
