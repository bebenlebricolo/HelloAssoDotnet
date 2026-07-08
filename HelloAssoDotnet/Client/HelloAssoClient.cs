using HelloAssoDotnet.Models.Configuration;
using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Provides a client for interacting with the HelloAsso API, enabling operations such as
/// authentication, retrieving payment details, fetching organization forms, and downloading receipts or event tickets.
/// This facade exposes the API surface through resource sub-clients. Authentication and the in-memory token
/// cache live on the shared <see cref="HelloAssoConnection"/>; every collaborator is injected by the DI
/// container (see <c>AddHelloAsso</c>) - the facade does not build anything itself.
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
    /// Instantiates the facade from its injected collaborators. Everything (the shared connection and the
    /// resource sub-clients) is resolved by the DI container; nothing is constructed here.
    /// </summary>
    /// <param name="connection">Shared connection owning authentication and the token cache.</param>
    /// <param name="organizations">Organizations sub-client.</param>
    /// <param name="forms">Forms sub-client.</param>
    /// <param name="orders">Orders sub-client.</param>
    /// <param name="items">Items sub-client.</param>
    /// <param name="payments">Payments sub-client.</param>
    /// <param name="checkout">Checkout sub-client.</param>
    /// <param name="directory">Directory sub-client.</param>
    /// <param name="partners">Partners sub-client.</param>
    /// <param name="users">Users sub-client.</param>
    /// <param name="values">Values sub-client.</param>
    /// <param name="cashOut">Cash-out sub-client.</param>
    /// <param name="notifications">Notifications sub-client.</param>
    public HelloAssoClient(HelloAssoConnection connection,
                           IOrganizationsClient organizations,
                           IFormsClient forms,
                           IOrdersClient orders,
                           IItemsClient items,
                           IPaymentsClient payments,
                           ICheckoutClient checkout,
                           IDirectoryClient directory,
                           IPartnersClient partners,
                           IUsersClient users,
                           IValuesClient values,
                           ICashOutClient cashOut,
                           INotificationsClient notifications)
    {
        _connection = connection;
        Organizations = organizations;
        Forms = forms;
        Orders = orders;
        Items = items;
        Payments = payments;
        Checkout = checkout;
        Directory = directory;
        Partners = partners;
        Users = users;
        Values = values;
        CashOut = cashOut;
        Notifications = notifications;
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
