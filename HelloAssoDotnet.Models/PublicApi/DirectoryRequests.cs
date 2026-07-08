using HelloAssoDotnet.Models.Api.Forms;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Filter body for the public directory forms search.
/// <see aref="https://dev.helloasso.com/reference/post_directory-forms"/>
/// </summary>
public record DirectoryFormsFilters
{
    /// <summary>
    /// Restrict to these form types. Leave empty for no filter.
    /// </summary>
    public List<FormType> FormTypes { get; set; } = new List<FormType>();

    /// <summary>
    /// Restrict to forms tagged with these public tags. Leave empty for no filter.
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();
}

/// <summary>
/// Request for the public directory forms search. Pagination for directory endpoints is done exclusively
/// through the continuation token: the API cannot return a total count (pagination totals are always -1).
/// </summary>
public record DirectoryFormsRequest
{
    /// <summary>
    /// Filter body sent to the endpoint.
    /// </summary>
    public DirectoryFormsFilters Filters { get; set; } = new DirectoryFormsFilters();

    /// <summary>
    /// Queried page size.
    /// </summary>
    public int? PageSize { get; set; }

    /// <summary>
    /// Continuation token used to iterate the directory.
    /// </summary>
    public string? ContinuationToken { get; set; }
}

/// <summary>
/// Filter body for the public directory organizations search.
/// <see aref="https://dev.helloasso.com/reference/post_directory-organizations"/>
/// </summary>
public record DirectoryOrganizationsFilters
{
    /// <summary>
    /// Restrict to organizations tagged with these public tags. Leave empty for no filter.
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();
}

/// <summary>
/// Request for the public directory organizations search. Pagination is done exclusively through the
/// continuation token (pagination totals are always -1).
/// </summary>
public record DirectoryOrganizationsRequest
{
    /// <summary>
    /// Filter body sent to the endpoint.
    /// </summary>
    public DirectoryOrganizationsFilters Filters { get; set; } = new DirectoryOrganizationsFilters();

    /// <summary>
    /// Queried page size.
    /// </summary>
    public int? PageSize { get; set; }

    /// <summary>
    /// Continuation token used to iterate the directory.
    /// </summary>
    public string? ContinuationToken { get; set; }
}
