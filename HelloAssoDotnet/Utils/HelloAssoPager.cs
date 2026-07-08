using System.Runtime.CompilerServices;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Utils;

/// <summary>
/// Generic auto-pager for HelloAsso listing endpoints. It follows the <c>continuationToken</c> returned in
/// <see cref="Models.HelloAssoApi.Base.PaginationProperties"/> until the API stops returning results.
/// This is a plain helper (no state, no HttpClient): sub-clients supply a page-fetching delegate.
/// </summary>
public static class HelloAssoPager
{
    /// <summary>
    /// Enumerates every element across all pages, transparently following the continuation token.
    /// Iteration stops on the first failing page, on an empty page, or once the API stops handing a new token.
    /// </summary>
    /// <typeparam name="TResponse">Concrete paginated response type</typeparam>
    /// <typeparam name="T">Element type contained in a page</typeparam>
    /// <param name="fetchPage">
    /// Delegate fetching a single page for the given continuation token (null for the first page).
    /// </param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An async stream of every element found across the pages.</returns>
    public static async IAsyncEnumerable<T> PageAllAsync<TResponse, T>(
        Func<string?, CancellationToken, Task<Result<TResponse>>> fetchPage,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TResponse : class, IPaginatedResponse<T>
    {
        string? continuationToken = null;
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var page = await fetchPage(continuationToken, cancellationToken);
            if (!page.IsOk)
            {
                yield break;
            }

            var data = page.Value!.Data;
            if (data.Count == 0)
            {
                yield break;
            }

            foreach (var item in data)
            {
                yield return item;
            }

            var nextToken = page.Value!.Pagination.ContinuationToken;
            // Stop when the API no longer hands a fresh token (end of the listing).
            if (string.IsNullOrEmpty(nextToken) || nextToken == continuationToken)
            {
                yield break;
            }
            continuationToken = nextToken;
        }
    }
}
