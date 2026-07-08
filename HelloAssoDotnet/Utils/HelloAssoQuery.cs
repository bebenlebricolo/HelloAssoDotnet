using HelloAssoDotnet.Models.HelloAssoApi.Forms;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Utils;

/// <summary>
/// Centralizes the (repetitive) query-string building for the various listing endpoints so that the
/// organization-scoped and form-scoped variants share the exact same parameter handling.
/// These are plain functions operating on strings, they do not send anything.
/// </summary>
public static class HelloAssoQuery
{
    /// <summary>
    /// Appends the organization forms listing filters (states, form types, pagination) to the URL.
    /// </summary>
    public static string BuildForms(string url, ListOrganizationFormsRequest request)
    {
        uint queriesCount = 0;
        foreach (var state in request.States)
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, state.ToString(), "states", ref queriesCount);
        }

        foreach (var formType in request.FormTypes)
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, formType.ToString(), "formTypes", ref queriesCount);
        }

        if (request.PageIndex != null)
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, request.PageIndex.ToString()!, "pageIndex", ref queriesCount);
        }

        if (request.PageSize != null)
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, request.PageSize.ToString()!, "pageSize", ref queriesCount);
        }

        if (request.ContinuationToken != null)
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, request.ContinuationToken, "continuationToken", ref queriesCount);
        }

        return url;
    }

    /// <summary>
    /// Appends the order listing filters (form types, dates, pagination) to the URL.
    /// </summary>
    public static string BuildOrders(string url, ListOrdersRequest request)
    {
        uint queriesCount = 0;
        foreach (var formType in request.FormTypes)
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, formType.ToString(), "formTypes", ref queriesCount);
        }

        url = AppendDateRangeAndPagination(url, request.From, request.To, request.PageIndex, request.PageSize, request.ContinuationToken, ref queriesCount);
        return url;
    }

    /// <summary>
    /// Appends the payment search filters (search key, states, dates, pagination) to the URL.
    /// </summary>
    public static string BuildPayments(string url, SearchPaymentsRequest request)
    {
        uint queriesCount = 0;
        if (!string.IsNullOrEmpty(request.UserSearchKey))
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, Uri.EscapeDataString(request.UserSearchKey), "userSearchKey", ref queriesCount);
        }

        foreach (var state in request.States)
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, state.ToString(), "states", ref queriesCount);
        }

        url = AppendDateRangeAndPagination(url, request.From, request.To, request.PageIndex, request.PageSize, request.ContinuationToken, ref queriesCount);
        return url;
    }

    /// <summary>
    /// Appends the item listing filters (states, dates, pagination) to the URL.
    /// </summary>
    public static string BuildItems(string url, ListItemsRequest request)
    {
        uint queriesCount = 0;
        foreach (var state in request.States)
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, state.ToString(), "states", ref queriesCount);
        }

        url = AppendDateRangeAndPagination(url, request.From, request.To, request.PageIndex, request.PageSize, request.ContinuationToken, ref queriesCount);
        return url;
    }

    private static string AppendDateRangeAndPagination(string url,
                                                       DateTime? from,
                                                       DateTime? to,
                                                       int? pageIndex,
                                                       int? pageSize,
                                                       string? continuationToken,
                                                       ref uint queriesCount)
    {
        if (from != null)
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, Uri.EscapeDataString(from.Value.ToString("o")), "from", ref queriesCount);
        }

        if (to != null)
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, Uri.EscapeDataString(to.Value.ToString("o")), "to", ref queriesCount);
        }

        if (pageIndex != null)
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, pageIndex.ToString()!, "pageIndex", ref queriesCount);
        }

        if (pageSize != null)
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, pageSize.ToString()!, "pageSize", ref queriesCount);
        }

        if (continuationToken != null)
        {
            url = HelloAssoHttpExtensions.AddQueryToUrl(url, continuationToken, "continuationToken", ref queriesCount);
        }

        return url;
    }
}
