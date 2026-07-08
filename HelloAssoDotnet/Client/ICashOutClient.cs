using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Read-only operations over cash-outs (money transfers to the organization).
/// </summary>
public interface ICashOutClient
{
    /// <summary>
    /// Retrieves the export (detailed content) of a cash-out for the configured organization.
    /// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-cash-out-cashoutid-export"/>
    /// </summary>
    /// <param name="cashOutId">The cash-out identifier.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The raw export stream.</returns>
    Task<Result<Stream>> GetExportAsync(string cashOutId, AuthTokens? tokens = null, CancellationToken cancellationToken = default);
}
