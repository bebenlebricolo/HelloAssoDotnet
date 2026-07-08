using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Base class shared by resource sub-clients. It only carries the shared <see cref="IHelloAssoClientContext"/>
/// and a helper to resolve the access token (either an explicit one supplied by the caller, or the token
/// cached on the root client). It does not send requests itself.
/// </summary>
internal abstract class HelloAssoSubClient
{
    /// <summary>
    /// Shared context (HttpClient, config, token access, ...) provided by the root client.
    /// </summary>
    protected readonly IHelloAssoClientContext Context;

    /// <summary>
    /// Builds a sub-client bound to the given root context.
    /// </summary>
    /// <param name="context">Shared root context</param>
    protected HelloAssoSubClient(IHelloAssoClientContext context)
    {
        Context = context;
    }

    /// <summary>
    /// Resolves the access token to use for a request. When the caller supplies explicit
    /// <paramref name="tokens"/> (the "grown-up" mode), they bypass the internal cache; otherwise the
    /// cached/auto-refreshed token from the root client is used.
    /// </summary>
    /// <param name="tokens">Optional explicit tokens supplied by the caller</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A result wrapping the access token string, or the mapped error.</returns>
    protected async Task<Result<string>> ResolveAccessTokenAsync(AuthTokens? tokens, CancellationToken cancellationToken)
    {
        if (tokens != null && !string.IsNullOrEmpty(tokens.AccessToken))
        {
            return Result<string>.Ok(tokens.AccessToken);
        }

        return await Context.GetValidAccessTokenAsync(cancellationToken);
    }
}
