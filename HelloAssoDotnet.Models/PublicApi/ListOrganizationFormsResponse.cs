using HelloAssoDotnet.Models.HelloAssoApi.Base;
using HelloAssoDotnet.Models.HelloAssoApi.Forms;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// </summary>
public record ListOrganizationFormsResponse : IPaginatedResponse<FormLightModel>
{
    /// <summary>
    /// List of retrieved forms
    /// </summary>
    public List<FormLightModel> Data { get; set; } =  new List<FormLightModel>();

    /// <summary>
    /// Pagination Properties
    /// </summary>
    public PaginationProperties Pagination { get; set; }  = new PaginationProperties();

    /// <inheritdoc />
    IReadOnlyList<FormLightModel> IPaginatedResponse<FormLightModel>.Data => Data;
}
