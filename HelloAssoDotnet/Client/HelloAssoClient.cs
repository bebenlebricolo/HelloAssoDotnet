using HelloAssoDotnet.Client;
using HelloAssoDotnet.Models.Configuration;
using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Provides a client for interacting with the HelloAsso API, enabling operations such as
/// authentication, retrieving payment details, fetching organization forms, and downloading receipts or event tickets.
/// This facade exposes the API surface through resource sub-clients. Authentication and the in-memory token
/// cache live on the shared <see cref="HelloAssoConnection"/>, which is created first and handed to every
/// sub-client; the facade does not wrap requests behind an executor.
/// </summary>
public class HelloAssoClient : IHelloAssoClient
{
    private readonly HelloAssoConnection _connection;

    /// <inheritdoc />
    public ClientConfig Config
    {
        get => _connection.Config;
        set => _connection.Config = value;
    }

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
        var appsettingsConfiguration = new AppsettingsConfiguration();
        appsettingsConfiguration.FromConfig(configuration);

        // Build the shared connection first so sub-clients receive a fully-constructed context (no leaked
        // partially-initialized "this").
        _connection = new HelloAssoConnection(httpClient, secretsService, logger, appsettingsConfiguration);

        // Sub-clients share this connection (via IHelloAssoClientContext) for config + token access.
        Organizations = new OrganizationsClient(_connection);
        Forms = new FormsClient(_connection);
        Orders = new OrdersClient(_connection);
        Items = new ItemsClient(_connection);
        Payments = new PaymentsClient(_connection);
        Checkout = new CheckoutClient(_connection);
        Directory = new DirectoryClient(_connection);
        Partners = new PartnersClient(_connection);
        Users = new UsersClient(_connection);
        Values = new ValuesClient(_connection);
        CashOut = new CashOutClient(_connection);
        Notifications = new NotificationsClient(_connection);
    }

    /// <inheritdoc />
    public Task<Result<AuthTokens>> AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        return _connection.AuthenticateAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<AuthTokens>> RefreshTokenAsync(AuthTokens tokens, CancellationToken cancellationToken = default)
    {
        return _connection.RefreshTokenAsync(tokens, cancellationToken);
    }
}
