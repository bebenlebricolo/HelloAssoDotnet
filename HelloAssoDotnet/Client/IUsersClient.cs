using HelloAssoDotnet.Models.HelloAssoApi.Auth;
using HelloAssoDotnet.Models.HelloAssoApi.Organizations;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Read-only operations about the currently authenticated user.
/// </summary>
public interface IUsersClient
{
    /// <summary>
    /// Returns the organizations on which the currently authenticated user has rights.
    /// <see aref="https://dev.helloasso.com/reference/get_users-me-organizations"/>
    /// </summary>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The organizations the user has rights on.</returns>
    Task<Result<List<OrganizationLightModel>>> GetMyOrganizationsAsync(AuthTokens? tokens = null, CancellationToken cancellationToken = default);
}
