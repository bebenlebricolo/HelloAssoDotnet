namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Search payment response from <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-payments"/>
/// </summary>
public record SearchPaymentResponse : PaginatedResponse<PaymentResponse>;
