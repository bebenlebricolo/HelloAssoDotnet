using HelloAssoDotnet.Models.Api.Organizations;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Paginated list of organizations, used by partner and user "my organizations" listings.
/// <see aref="https://dev.helloasso.com/reference/get_partners-me-organizations"/>
/// </summary>
public record ListOrganizationsResponse : PaginatedResponse<OrganizationLightModel>;
