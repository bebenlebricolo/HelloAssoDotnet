using HelloAssoDotnet.Models.HelloAssoApi.Base;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Search payment response from <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-payments"/>
/// </summary>
public record SearchPaymentResponse
{
    /// <summary>
    /// Actual data list
    /// </summary>
    public List<PaymentResponse> Data { get; set; } = new List<PaymentResponse>();

    /// <summary>
    /// Pagination properties retrieved from listing endpoints
    /// </summary>
    public PaginationProperties Pagination { get; set; } = new PaginationProperties();
}
