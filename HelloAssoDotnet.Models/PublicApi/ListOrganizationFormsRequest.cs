using HelloAssoDotnet.Models.HelloAssoApi.Forms;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Note : if all queries are left to blank, HelloAsso Api
/// uses the default values for each of them.
/// </summary>
public record ListOrganizationFormsRequest
{
    /// <summary>
    /// List of publication states filters
    /// Leave uninitialized (blank) to get everything
    /// </summary>
    public List<PublicationState> States  { get; set; } = new List<PublicationState>();

    /// <summary>
    /// List of queried form types.
    /// Leave uninitialized (blank) to get everything
    /// </summary>
    public List<FormType> FormTypes { get; set; } = new List<FormType>();

    /// <summary>
    /// Queried page index
    /// </summary>
    public int? PageIndex  { get; set; }

    /// <summary>
    /// Queried page size
    /// </summary>
    public int? PageSize { get; set; }

    /// <summary>
    /// Continuation token from which we wish to retrieve results.
    /// One can issue a first query (with page index and page size, form type) and then continue sending
    /// the continuationToken which will then have the full context of previous requests and handle page iteration
    /// on its own.
    /// </summary>
    /// <example>
    /// Request #1 : get all forms of type events, page size = 20 and page index = 1 (1 - based counting)
    /// Request #2 : using the continuationToken, simply resupply the continuationToken with no further parameters
    /// Request #n : continue cycling the requests until reaching the point where the returned data[] payload is empty.
    /// </example>
    public string? ContinuationToken { get; set; }
}
