using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.Api.Organizations;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Read-only operations scoped to the configured organization.
/// </summary>
public interface IOrganizationsClient
{
    /// <summary>
    /// Retrieves the public information of the configured organization.
    /// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug"/>
    /// </summary>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The organization public details.</returns>
    Task<Result<OrganizationDetails>> GetAsync(AuthTokens? tokens = null, CancellationToken cancellationToken = default);
}
