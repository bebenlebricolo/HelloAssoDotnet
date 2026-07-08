using System.Text.Json;
using System.Text.Json.Serialization;
using HelloAssoDotnet.Client.SubClients;
using HelloAssoDotnet.Models.Configuration;
using HelloAssoDotnet.Models.HelloAssoApi.Auth;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Services;
using HelloAssoDotnet.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Provides a client for interacting with the HelloAsso API, enabling operations such as
/// authentication, retrieving payment details, fetching organization forms, and downloading receipts or event tickets.
/// This root node owns the OAuth tokens (with in-memory caching + auto-refresh) and exposes the API surface
/// through resource sub-clients. It does not wrap requests behind an executor: each sub-client uses the shared
/// <see cref="HttpClient"/> directly.
/// </summary>
public class HelloAssoClient : IHelloAssoClient, IHelloAssoClientContext
{
    private readonly HttpClient _httpClient;
    private readonly Uri _baseUri;
    private readonly Uri _oauthEndpoint;
    private readonly ILogger<HelloAssoClient> _logger;
    private readonly IHelloAssoSecretsService _secretsService;
    private readonly AppsettingsConfiguration _appsettingsConfiguration;

    /// <summary>
    /// Safety margin (seconds) applied before a cached token is considered expired, so we renew slightly early.
    /// </summary>
    private const int TokenExpirySkewSeconds = 60;

    private AuthTokens? _cachedTokens;
    private DateTimeOffset _tokenExpiryUtc = DateTimeOffset.MinValue;

    /// <summary>
    /// Used to provide user agent details and other parameters that can only be provided by the calling layer
    /// </summary>
    public ClientConfig Config { get; set; } = new ClientConfig()
    {
        UserAgent = "HelloAssoDotnetClient",
        UserAgentVersion = "1.0.0",
    };

    private string _clientId = "";
    private string _clientSecret = "";

    /// <inheritdoc />
    public IOrganizationsClient Organizations { get; }

    /// <inheritdoc />
    public IFormsClient Forms { get; }

    /// <inheritdoc />
    public IOrdersClient Orders { get; }

    /// <inheritdoc />
    public IItemsClient Items { get; }

    /// <inheritdoc />
    public IPaymentsClient Payments { get; }

    /// <inheritdoc />
    public ICheckoutClient Checkout { get; }

    /// <inheritdoc />
    public IDirectoryClient Directory { get; }

    /// <inheritdoc />
    public IPartnersClient Partners { get; }

    /// <inheritdoc />
    public IUsersClient Users { get; }

    /// <inheritdoc />
    public IValuesClient Values { get; }

    /// <inheritdoc />
    public ICashOutClient CashOut { get; }

    /// <inheritdoc />
    public INotificationsClient Notifications { get; }

    /// <summary>
    /// Instantiates a basic HelloAssoClient
    /// </summary>
    /// <param name="httpClient">HttpClient used under the hood</param>
    /// <param name="secretsService">Secret service is used to retrieve client id/ client secrets pair</param>
    /// <param name="logger">Logger</param>
    /// <param name="configuration">Static configuration, pulled from appsettings.</param>
    public HelloAssoClient(HttpClient httpClient,
                           IHelloAssoSecretsService secretsService,
                           ILogger<HelloAssoClient> logger,
                           IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _secretsService = secretsService;

        _appsettingsConfiguration = new AppsettingsConfiguration();
        _appsettingsConfiguration.FromConfig(configuration);

        // Resolve the API + OAuth endpoints from the configured environment (production vs sandbox).
        _baseUri = HelloAssoEnvironmentUris.GetApiBaseUri(_appsettingsConfiguration.Environment);
        _oauthEndpoint = HelloAssoEnvironmentUris.GetOauthTokenUri(_appsettingsConfiguration.Environment);

        // Sub-clients share this root node (via IHelloAssoClientContext) for config + token access.
        Organizations = new OrganizationsClient(this);
        Forms = new FormsClient(this);
        Orders = new OrdersClient(this);
        Items = new ItemsClient(this);
        Payments = new PaymentsClient(this);
        Checkout = new CheckoutClient(this);
        Directory = new DirectoryClient(this);
        Partners = new PartnersClient(this);
        Users = new UsersClient(this);
        Values = new ValuesClient(this);
        CashOut = new CashOutClient(this);
        Notifications = new NotificationsClient(this);
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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
    /// Returns a valid access token: reuses the cached one, refreshes it if possible, or re-authenticates.
    /// </summary>
    private async Task<Result<string>> GetValidAccessTokenInternalAsync(CancellationToken cancellationToken)
    {
        if (_cachedTokens != null && DateTimeOffset.UtcNow < _tokenExpiryUtc && !string.IsNullOrEmpty(_cachedTokens.AccessToken))
        {
            return Result<string>.Ok(_cachedTokens.AccessToken);
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

    // --- IHelloAssoClientContext (shared with sub-clients; explicit so it stays off the public surface) ---

    HttpClient IHelloAssoClientContext.HttpClient => _httpClient;

    Uri IHelloAssoClientContext.BaseUri => _baseUri;

    string IHelloAssoClientContext.OrganizationSlug => _appsettingsConfiguration.OrganizationSlug;

    ClientConfig IHelloAssoClientContext.Config => Config;

    ILogger IHelloAssoClientContext.Logger => _logger;

    Task<Result<string>> IHelloAssoTokenAccessor.GetValidAccessTokenAsync(CancellationToken cancellationToken) => GetValidAccessTokenInternalAsync(cancellationToken);
}
