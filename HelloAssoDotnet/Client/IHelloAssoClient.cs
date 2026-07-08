using HelloAssoDotnet.Models.Configuration;
using HelloAssoDotnet.Models.HelloAssoApi.Auth;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Root entry point for the HelloAsso API. It owns authentication and the cached access token, and exposes
/// the API surface grouped into resource sub-clients (e.g. <c>client.Orders.GetAsync(...)</c>).
/// Sub-client methods use the cached token automatically; pass an explicit <see cref="AuthTokens"/> to any
/// of them to manage tokens yourself.
/// </summary>
public interface IHelloAssoClient
{
    /// <summary>
    /// Authenticate the webserver application (client_credentials grant).
    /// On success, the returned tokens are cached and reused by the sub-clients until they near expiry.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The fresh authentication tokens.</returns>
    Task<Result<AuthTokens>> AuthenticateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the given Jwt token (refresh_token grant). On success the cache is updated.
    /// </summary>
    /// <param name="tokens">Tokens carrying the refresh token to use.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The refreshed authentication tokens.</returns>
    Task<Result<AuthTokens>> RefreshTokenAsync(AuthTokens tokens, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the client configuration (used by calling layers, e.g. the user agent).
    /// </summary>
    ClientConfig Config { get; set; }

    /// <summary>
    /// Operations scoped to the configured organization.
    /// </summary>
    IOrganizationsClient Organizations { get; }

    /// <summary>
    /// Operations over the forms (campaigns) of the configured organization.
    /// </summary>
    IFormsClient Forms { get; }

    /// <summary>
    /// Operations over orders.
    /// </summary>
    IOrdersClient Orders { get; }

    /// <summary>
    /// Operations over items (individual entries sold within orders).
    /// </summary>
    IItemsClient Items { get; }

    /// <summary>
    /// Operations over payments.
    /// </summary>
    IPaymentsClient Payments { get; }

    /// <summary>
    /// Read-only operations over HelloAsso Checkout.
    /// </summary>
    ICheckoutClient Checkout { get; }

    /// <summary>
    /// Public directory searches.
    /// </summary>
    IDirectoryClient Directory { get; }

    /// <summary>
    /// Operations about the calling partner.
    /// </summary>
    IPartnersClient Partners { get; }

    /// <summary>
    /// Operations about the currently authenticated user.
    /// </summary>
    IUsersClient Users { get; }

    /// <summary>
    /// Reference data ("values").
    /// </summary>
    IValuesClient Values { get; }

    /// <summary>
    /// Operations over cash-outs.
    /// </summary>
    ICashOutClient CashOut { get; }

    /// <summary>
    /// Helpers to consume HelloAsso notifications (webhooks).
    /// </summary>
    INotificationsClient Notifications { get; }
}
