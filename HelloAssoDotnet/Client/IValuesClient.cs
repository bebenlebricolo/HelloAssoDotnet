using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.Api.Forms;
using HelloAssoDotnet.Models.Api.Values;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Read-only reference data ("values") used to build dropdowns and categorize entities.
/// </summary>
public interface IValuesClient
{
    /// <summary>
    /// Retrieves the list of company legal statuses.
    /// <see aref="https://dev.helloasso.com/reference/get_values-company-legal-status"/>
    /// </summary>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The legal statuses.</returns>
    Task<Result<List<CompanyLegalStatus>>> GetCompanyLegalStatusesAsync(AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the list of organization categories (Journal Officiel).
    /// <see aref="https://dev.helloasso.com/reference/get_values-organization-categories"/>
    /// </summary>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The organization categories.</returns>
    Task<Result<List<OrganizationCategory>>> GetOrganizationCategoriesAsync(AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the list of public tags.
    /// <see aref="https://dev.helloasso.com/reference/get_values-tags"/>
    /// </summary>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The public tags.</returns>
    Task<Result<List<PublicTag>>> GetTagsAsync(AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the list of activity sub-types available for a given form type.
    /// <see aref="https://dev.helloasso.com/reference/get_values-form-formtype-types"/>
    /// </summary>
    /// <param name="formType">Form type whose sub-types are requested.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The form sub-types.</returns>
    Task<Result<List<FormSubType>>> GetFormSubTypesAsync(FormType formType, AuthTokens? tokens = null, CancellationToken cancellationToken = default);
}
