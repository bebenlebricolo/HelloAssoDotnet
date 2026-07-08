using System.Text.Json;
using System.Text.Json.Serialization;
using HelloAssoDotnet.Models.Configuration;
using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Services;
using HelloAssoDotnet.Utils;
using Microsoft.Extensions.Logging;

namespace HelloAssoDotnet.Client;

/// <summary>
/// The shared root node behind <see cref="HelloAssoClient"/>. It owns the state every sub-client needs
/// (the <see cref="HttpClient"/>, configuration, resolved base URI, logger) and the OAuth token cache with
/// auto-refresh. It is resolved once from the DI container and handed to every sub-client, so there is a
/// single shared token cache.
/// It is NOT a request-executing layer: sub-clients still build and send their own requests directly.
/// </summary>
public class HelloAssoConnection : IHelloAssoClientContext
{
    /// <summary>
    /// Name of the <see cref="System.Net.Http.HttpClient"/> registered for the HelloAsso connection. Tests and
    /// advanced callers can configure this named client (e.g. the primary message handler) through the usual
    /// <c>IHttpClientFactory</c> configuration.
    /// </summary>
    public const string HttpClientName = "HelloAsso";

    private readonly HttpClient _httpClient;
    private readonly Uri _baseUri;
    private readonly Uri _oauthEndpoint;
    private readonly ILogger _logger;
    private readonly IHelloAssoSecretsService _secretsService;
    private readonly AppsettingsConfiguration _appsettingsConfiguration;

    /// <summary>
    /// Safety margin (seconds) applied before a cached token is considered expired, so we renew slightly early.
    /// </summary>
    private const int TokenExpirySkewSeconds = 60;

    private AuthTokens? _cachedTokens;
    private DateTimeOffset _tokenExpiryUtc = DateTimeOffset.MinValue;

    /// <summary>
    /// Serializes token authentication/refresh so concurrent sub-client calls trigger a single round-trip
    /// (and share the resulting token) instead of each racing to authenticate.
    /// </summary>
    private readonly SemaphoreSlim _tokenLock = new SemaphoreSlim(1, 1);

    private string _clientId = "";
    private string _clientSecret = "";

    /// <summary>
    /// Used to provide user agent details and other parameters that can only be provided by the calling layer.
    /// </summary>
    public ClientConfig Config { get; set; } = new ClientConfig()
    {
        UserAgent = "HelloAssoDotnetClient",
        UserAgentVersion = "1.0.0",
    };

    /// <summary>
    /// Builds the connection, resolving the API + OAuth endpoints from the configured URLs.
    /// </summary>
    /// <param name="httpClientFactory">Factory used to create the shared named HttpClient.</param>
    /// <param name="secretsService">Secret service used to retrieve the client id / client secret pair.</param>
    /// <param name="logger">Logger.</param>
    /// <param name="appsettingsConfiguration">Static configuration, pulled from appsettings.</param>
    public HelloAssoConnection(IHttpClientFactory httpClientFactory,
                               IHelloAssoSecretsService secretsService,
                               ILogger<HelloAssoConnection> logger,
                               AppsettingsConfiguration appsettingsConfiguration)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientName);
        _secretsService = secretsService;
        _logger = logger;
        _appsettingsConfiguration = appsettingsConfiguration;

        // API + OAuth endpoints come from configuration (defaulting to the URLs of the selected environment).
        _baseUri = new Uri(_appsettingsConfiguration.ApiBaseUrl);
        _oauthEndpoint = new Uri(_appsettingsConfiguration.OauthTokenUrl);
    }

    /// <summary>
    /// Retrieve HelloAsso secrets from the secrets service.
    /// </summary>
    /// <returns></returns>
    public bool RetrieveSecrets()
    {
        if (_clientId != "" && _clientSecret != "")
        {
            return true;
        }

        _clientId = _secretsService.GetClientId() ?? "";
        _clientSecret = _secretsService.GetClientSecret() ?? "";
        if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_clientSecret))
        {
            _logger.LogError("Cannot read secrets for HelloAsso client.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Authenticate the webserver application (client_credentials grant). On success the tokens are cached.
    /// </summary>
    public async Task<Result<AuthTokens>> AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        if (!RetrieveSecrets())
        {
            return Result<AuthTokens>.FromError(Errors.ClientError);
        }

        var message = new HttpRequestMessage(HttpMethod.Post, _oauthEndpoint);
        var payload = new Dictionary<string, string>
        {
            { "client_id", _clientId },
            { "client_secret", _clientSecret },
            { "grant_type", "client_credentials" },
        };

        message.Content = new FormUrlEncodedContent(payload);
        message.WithUserAgent(Config);
        var response = await _httpClient.SendAsync(message, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to authenticate to HelloAsso API");
            return Result<AuthTokens>.FromHttpResponse(response);
        }

        var content = await JsonSerializer.DeserializeAsync<AuthTokens>(await response.Content.ReadAsStreamAsync(cancellationToken), GetTokenJsonOptions(), cancellationToken);
        if (content == null)
        {
            return Result<AuthTokens>.FromError(Errors.ClientError);
        }

        CacheTokens(content);
        return Result<AuthTokens>.Ok(content);
    }

    /// <summary>
    /// Custom json options for HelloAsso OAuth2 Token parsing
    /// </summary>
    /// <returns></returns>
    private static JsonSerializerOptions GetTokenJsonOptions()
    {
        return new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Refreshes the given Jwt token (refresh_token grant). On success the cache is updated.
    /// </summary>
    public async Task<Result<AuthTokens>> RefreshTokenAsync(AuthTokens tokens, CancellationToken cancellationToken = default)
    {
        if (!RetrieveSecrets())
        {
            return Result<AuthTokens>.FromError(Errors.UnknownError);
        }

        // Use the refresh token method
        var refreshRequestMessage = new HttpRequestMessage(HttpMethod.Post, _oauthEndpoint);
        var refreshPayload = new Dictionary<string, string>
        {
            { "client_id", _clientId },
            { "grant_type", "refresh_token" },
            { "refresh_token", tokens.RefreshToken },
        };
        refreshRequestMessage.Content = new FormUrlEncodedContent(refreshPayload);
        refreshRequestMessage.WithUserAgent(Config);
        var refreshResponse = await _httpClient.SendAsync(refreshRequestMessage, cancellationToken);
        if (!refreshResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to authenticate to HelloAsso API");
            return Result<AuthTokens>.FromHttpResponse(refreshResponse);
        }

        var refreshedToken = await JsonSerializer.DeserializeAsync<AuthTokens>(await refreshResponse.Content.ReadAsStreamAsync(cancellationToken), GetTokenJsonOptions(), cancellationToken);
        if (refreshedToken == null)
        {
            return new Result<AuthTokens>(Errors.ClientError);
        }

        CacheTokens(refreshedToken);
        return new Result<AuthTokens>(refreshedToken);
    }

    /// <summary>
    /// Stores the tokens in the in-memory cache and computes their (early) expiry.
    /// </summary>
    private void CacheTokens(AuthTokens tokens)
    {
        _cachedTokens = tokens;
        var lifetimeSeconds = Math.Max(0, tokens.ExpiresIn - TokenExpirySkewSeconds);
        _tokenExpiryUtc = DateTimeOffset.UtcNow.AddSeconds(lifetimeSeconds);
    }

    /// <summary>
    /// Whether the currently cached token can still be used (present and not past its (early) expiry).
    /// </summary>
    private bool IsCachedTokenValid()
    {
        return _cachedTokens != null && DateTimeOffset.UtcNow < _tokenExpiryUtc && !string.IsNullOrEmpty(_cachedTokens.AccessToken);
    }

    // --- IHelloAssoClientContext (shared with sub-clients) ---

    /// <inheritdoc />
    public HttpClient HttpClient => _httpClient;

    /// <inheritdoc />
    public Uri BaseUri => _baseUri;

    /// <inheritdoc />
    public string OrganizationSlug => _appsettingsConfiguration.OrganizationSlug;

    /// <inheritdoc />
    ClientConfig IHelloAssoClientContext.Config => Config;

    /// <inheritdoc />
    public ILogger Logger => _logger;

    /// <inheritdoc />
    public async Task<Result<string>> GetValidAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        // Fast path: a still-valid cached token needs no synchronization.
        if (IsCachedTokenValid())
        {
            return Result<string>.Ok(_cachedTokens!.AccessToken);
        }

        await _tokenLock.WaitAsync(cancellationToken);
        try
        {
            // Re-check inside the lock: another caller may have refreshed the token while we waited.
            if (IsCachedTokenValid())
            {
                return Result<string>.Ok(_cachedTokens!.AccessToken);
            }

            // We have an (expired) token with a refresh token: try to refresh first.
            if (_cachedTokens != null && !string.IsNullOrEmpty(_cachedTokens.RefreshToken))
            {
                var refreshed = await RefreshTokenAsync(_cachedTokens, cancellationToken);
                if (refreshed.IsOk)
                {
                    return Result<string>.Ok(refreshed.Value!.AccessToken);
                }
            }

            var authenticated = await AuthenticateAsync(cancellationToken);
            if (!authenticated.IsOk)
            {
                return Result<string>.FromError(authenticated.Error);
            }
            return Result<string>.Ok(authenticated.Value!.AccessToken);
        }
        finally
        {
            _tokenLock.Release();
        }
    }
}
