using HelloAssoDotnet.Models.Configuration;
using HelloAssoDotnet.Models.PublicApi;
using Microsoft.Extensions.Logging;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Minimal contract used by the root <see cref="HelloAssoClient"/> to lend a valid access token to its
/// sub-clients. It only exposes token retrieval: there is deliberately no request-executing layer.
/// </summary>
internal interface IHelloAssoTokenAccessor
{
    /// <summary>
    /// Returns a valid bearer access token, transparently authenticating or refreshing when the cached token
    /// is missing or about to expire.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A result wrapping the access token string, or the mapped error.</returns>
    Task<Result<string>> GetValidAccessTokenAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Everything the root client shares with its sub-clients. This is the root node exposing its own state
/// (HttpClient, config, base URI, logger) and the cached token; it is NOT a mediating request layer -
/// sub-clients still build and send their own requests using <see cref="HttpClient"/> directly.
/// </summary>
internal interface IHelloAssoClientContext : IHelloAssoTokenAccessor
{
    /// <summary>
    /// The HttpClient owned by the root client and reused by every sub-client.
    /// </summary>
    HttpClient HttpClient { get; }

    /// <summary>
    /// Resolved API base URI (depends on the configured environment).
    /// </summary>
    Uri BaseUri { get; }

    /// <summary>
    /// Organization slug taken from configuration.
    /// </summary>
    string OrganizationSlug { get; }

    /// <summary>
    /// Client configuration (user agent, ...).
    /// </summary>
    ClientConfig Config { get; }

    /// <summary>
    /// Logger used across the sub-clients.
    /// </summary>
    ILogger Logger { get; }
}
