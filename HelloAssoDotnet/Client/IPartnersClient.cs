using HelloAssoDotnet.Models.HelloAssoApi.Auth;
using HelloAssoDotnet.Models.HelloAssoApi.Partners;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Read-only operations about the calling partner and its linked organizations.
/// </summary>
public interface IPartnersClient
{
    /// <summary>
    /// Retrieves information about the calling partner.
    /// <see aref="https://dev.helloasso.com/reference/get_partners-me"/>
    /// </summary>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The partner information.</returns>
    Task<Result<PartnerInfo>> GetMeAsync(AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the organizations linked to the partner.
    /// <see aref="https://dev.helloasso.com/reference/get_partners-me-organizations"/>
    /// </summary>
    /// <param name="continuationToken">Continuation token for pagination.</param>
    /// <param name="pageSize">Queried page size.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A single page of organizations.</returns>
    Task<Result<ListOrganizationsResponse>> GetOrganizationsAsync(string? continuationToken = null, int? pageSize = null, AuthTokens? tokens = null, CancellationToken cancellationToken = default);
}
