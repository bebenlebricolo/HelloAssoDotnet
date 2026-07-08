using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.Api.Forms;
using HelloAssoDotnet.Models.Api.Organizations;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Public directory searches. These are POST endpoints but semantically read-only (they return listings).
/// Pagination is done exclusively through the continuation token (totals are always -1).
/// </summary>
public interface IDirectoryClient
{
    /// <summary>
    /// Searches the public directory of forms.
    /// <see aref="https://dev.helloasso.com/reference/post_directory-forms"/>
    /// </summary>
    /// <param name="request">Filters and continuation token.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A single page of forms.</returns>
    Task<Result<PaginatedResponse<FormLightModel>>> SearchFormsAsync(DirectoryFormsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches the public directory of organizations.
    /// <see aref="https://dev.helloasso.com/reference/post_directory-organizations"/>
    /// </summary>
    /// <param name="request">Filters and continuation token.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A single page of organizations.</returns>
    Task<Result<PaginatedResponse<OrganizationLightModel>>> SearchOrganizationsAsync(DirectoryOrganizationsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default);
}
