using HelloAssoDotnet.Models.HelloAssoApi.Base;
using HelloAssoDotnet.Models.HelloAssoApi.Organizations;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Paginated list of organizations, used by partner and user "my organizations" listings.
/// <see aref="https://dev.helloasso.com/reference/get_partners-me-organizations"/>
/// </summary>
public record ListOrganizationsResponse : IPaginatedResponse<OrganizationLightModel>
{
    /// <summary>
    /// Retrieved organizations for the current page.
    /// </summary>
    public List<OrganizationLightModel> Data { get; set; } = new List<OrganizationLightModel>();

    /// <summary>
    /// Pagination properties (totals may be -1 for partner listings).
    /// </summary>
    public PaginationProperties Pagination { get; set; } = new PaginationProperties();

    /// <inheritdoc />
    IReadOnlyList<OrganizationLightModel> IPaginatedResponse<OrganizationLightModel>.Data => Data;
}
