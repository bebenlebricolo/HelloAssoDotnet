using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.Api.Notifications;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Helpers to consume HelloAsso notifications (webhooks): parse the raw body and verify its authenticity.
/// </summary>
public interface INotificationsClient
{
    /// <summary>
    /// Parses a raw notification body into a <see cref="HelloAssoNotification"/>.
    /// </summary>
    /// <param name="rawBody">Raw JSON body received on the webhook endpoint.</param>
    /// <returns>The parsed notification, or a client error if the body could not be parsed.</returns>
    Result<HelloAssoNotification> Parse(string rawBody);

    /// <summary>
    /// Projects a parsed notification into a strongly-typed payload, chosen explicitly from
    /// <see cref="HelloAssoNotification.EventType"/>. Returns <c>null</c> for unknown event types or when the
    /// raw <see cref="HelloAssoNotification.Data"/> does not match the known model - in which case the caller
    /// can still read <see cref="HelloAssoNotification.Data"/> by hand.
    /// </summary>
    /// <param name="notification">The parsed notification whose <c>Data</c> should be typed.</param>
    /// <returns>The typed payload, or <c>null</c> when it cannot be produced.</returns>
    NotificationPayload? ReadData(HelloAssoNotification notification);

    /// <summary>
    /// Verifies the authenticity of a notification.
    /// HelloAsso does not sign notification payloads; the recommended approach is to re-fetch the referenced
    /// resource from the API and confirm it exists (and matches). This is what this method does.
    /// <see aref="https://dev.helloasso.com/docs/secure-webhook"/>
    /// </summary>
    /// <param name="notification">The parsed notification to verify.</param>
    /// <param name="tokens">Optional explicit tokens. When null, the cached token is used.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The verification outcome.</returns>
    Task<Result<NotificationVerification>> VerifyAuthenticityAsync(HelloAssoNotification notification, AuthTokens? tokens = null, CancellationToken cancellationToken = default);
}
