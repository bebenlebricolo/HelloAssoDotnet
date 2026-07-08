using HelloAssoDotnet.Models.HelloAssoApi.Base;
using HelloAssoDotnet.Models.HelloAssoApi.Forms;
using HelloAssoDotnet.Models.HelloAssoApi.Organizations;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Paginated list of forms returned by the public directory forms search.
/// </summary>
public record DirectoryFormsResponse : IPaginatedResponse<FormLightModel>
{
    /// <summary>
    /// Retrieved forms for the current page.
    /// </summary>
    public List<FormLightModel> Data { get; set; } = new List<FormLightModel>();

    /// <summary>
    /// Pagination properties (totals are always -1 for directory endpoints).
    /// </summary>
    public PaginationProperties Pagination { get; set; } = new PaginationProperties();

    /// <inheritdoc />
    IReadOnlyList<FormLightModel> IPaginatedResponse<FormLightModel>.Data => Data;
}

/// <summary>
/// Paginated list of organizations returned by the public directory organizations search.
/// </summary>
public record DirectoryOrganizationsResponse : IPaginatedResponse<OrganizationLightModel>
{
    /// <summary>
    /// Retrieved organizations for the current page.
    /// </summary>
    public List<OrganizationLightModel> Data { get; set; } = new List<OrganizationLightModel>();

    /// <summary>
    /// Pagination properties (totals are always -1 for directory endpoints).
    /// </summary>
    public PaginationProperties Pagination { get; set; } = new PaginationProperties();

    /// <inheritdoc />
    IReadOnlyList<OrganizationLightModel> IPaginatedResponse<OrganizationLightModel>.Data => Data;
}
