using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.Api.Forms;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Read-only operations over the forms (campaigns) of the configured organization.
/// </summary>
public interface IFormsClient
{
    /// <summary>
    /// Retrieves a list of all forms for the configured organization using the provided request model.
    /// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-forms"/>
    /// </summary>
    /// <param name="request">Request model (form types, states, pagination).</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A single page of forms.</returns>
    Task<Result<ListOrganizationFormsResponse>> ListAsync(ListOrganizationFormsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enumerates every form of the configured organization, transparently following the continuation token.
    /// </summary>
    /// <param name="request">Filtering parameters (pagination is handled automatically).</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An async stream of forms.</returns>
    IAsyncEnumerable<FormLightModel> ListAllAsync(ListOrganizationFormsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single form's public details.
    /// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-forms-formtype-formslug-public"/>
    /// </summary>
    /// <param name="formSlug">Exact form slug.</param>
    /// <param name="formType">Form type.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The form public details.</returns>
    Task<Result<FormDetails>> GetPublicDetailsAsync(string formSlug, FormType formType, AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the form types for which the organization has at least one form.
    /// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-formtypes"/>
    /// </summary>
    /// <param name="states">Optional publication state filters.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The form types present in the organization.</returns>
    Task<Result<List<FormType>>> GetTypesAsync(IEnumerable<PublicationState>? states = null, AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the items (sold entries) of a form.
    /// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-forms-formtype-formslug-items"/>
    /// </summary>
    /// <param name="formType">Form type.</param>
    /// <param name="formSlug">Exact form slug.</param>
    /// <param name="request">Filtering and pagination parameters.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A single page of items.</returns>
    Task<Result<ListItemsResponse>> GetItemsAsync(FormType formType, string formSlug, ListItemsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the orders placed on a form.
    /// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-forms-formtype-formslug-orders"/>
    /// </summary>
    /// <param name="formType">Form type.</param>
    /// <param name="formSlug">Exact form slug.</param>
    /// <param name="request">Filtering and pagination parameters.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A single page of orders.</returns>
    Task<Result<ListOrdersResponse>> GetOrdersAsync(FormType formType, string formSlug, ListOrdersRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the payments made on a form.
    /// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-forms-formtype-formslug-payments"/>
    /// </summary>
    /// <param name="formType">Form type.</param>
    /// <param name="formSlug">Exact form slug.</param>
    /// <param name="request">Filtering and pagination parameters.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A single page of payments.</returns>
    Task<Result<SearchPaymentResponse>> GetPaymentsAsync(FormType formType, string formSlug, SearchPaymentsRequest request, AuthTokens? tokens = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves aggregated statistics for a form.
    /// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-forms-formtype-formslug-stats"/>
    /// </summary>
    /// <param name="formType">Form type.</param>
    /// <param name="formSlug">Exact form slug.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The form statistics.</returns>
    Task<Result<FormStats>> GetStatsAsync(FormType formType, string formSlug, AuthTokens? tokens = null, CancellationToken cancellationToken = default);
}
