using System.Text.Json;
using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.Api.Notifications;
using HelloAssoDotnet.Models.Api.Order;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Utils;
using Microsoft.Extensions.Logging;

namespace HelloAssoDotnet.Client;

/// <inheritdoc cref="INotificationsClient" />
internal sealed class NotificationsClient : HelloAssoSubClient, INotificationsClient
{
    public NotificationsClient(IHelloAssoClientContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public Result<HelloAssoNotification> Parse(string rawBody)
    {
        if (string.IsNullOrWhiteSpace(rawBody))
        {
            return Result<HelloAssoNotification>.FromError(Errors.ClientError);
        }

        try
        {
            using var document = JsonDocument.Parse(rawBody);
            var root = document.RootElement;

            var eventType = NotificationEventType.Unknown;
            if (root.TryGetProperty("eventType", out var eventTypeElement) && eventTypeElement.ValueKind == JsonValueKind.String)
            {
                Enum.TryParse(eventTypeElement.GetString(), ignoreCase: true, out eventType);
            }

            // Clone so the elements survive the JsonDocument being disposed.
            JsonElement data = default;
            if (root.TryGetProperty("data", out var dataElement))
            {
                data = dataElement.Clone();
            }

            JsonElement? metadata = null;
            if (root.TryGetProperty("metadata", out var metadataElement))
            {
                metadata = metadataElement.Clone();
            }

            var notification = new HelloAssoNotification
            {
                EventType = eventType,
                Data = data,
                Metadata = metadata,
            };
            return Result<HelloAssoNotification>.Ok(notification);
        }
        catch (JsonException e)
        {
            Context.Logger.LogError(e, "Failed to parse HelloAsso notification body.");
            return Result<HelloAssoNotification>.FromError(Errors.ClientError);
        }
    }

    /// <inheritdoc />
    public NotificationPayload? ReadData(HelloAssoNotification notification)
    {
        // HelloAsso webhook payloads are polymorphic: the shape of "data" is decided by "eventType".
        // This shape is NOT described by the OpenAPI spec, so we cannot rely on a generated/attribute-based
        // deserializer. Instead we branch explicitly on the event type and deserialize the raw JsonElement
        // into the matching hand-written model. This is intentionally 100% explicit (no [JsonPolymorphic],
        // no reflection): the resolution path is right here and easy to follow/debug. If the event type is
        // unknown (or the payload does not match the known model), we return null and the caller keeps the
        // raw notification.Data as an escape hatch.
        return notification.EventType switch
        {
            NotificationEventType.Payment when TryDeserialize<PaymentResponse>(notification.Data, out var payment)
                => new PaymentNotificationPayload(payment!),
            NotificationEventType.Order when TryDeserialize<OrderDetails>(notification.Data, out var order)
                => new OrderNotificationPayload(order!),
            NotificationEventType.Form when TryDeserialize<FormNotificationData>(notification.Data, out var form)
                => new FormNotificationPayload(form!),
            NotificationEventType.Organization when TryDeserialize<OrganizationNotificationData>(notification.Data, out var organization)
                => new OrganizationNotificationPayload(organization!),
            _ => null,
        };
    }

    /// <summary>
    /// Deserializes a raw notification <see cref="JsonElement"/> into a known model. Any parsing failure is
    /// logged and turned into a <c>false</c> result so the polymorphic switch can fall back gracefully.
    /// </summary>
    private bool TryDeserialize<T>(JsonElement data, out T? value) where T : class
    {
        value = null;
        if (data.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        try
        {
            value = data.Deserialize<T>(JsonOptionsProvider.GetJsonOptions());
            return value != null;
        }
        catch (JsonException e)
        {
            Context.Logger.LogError(e, "Failed to deserialize HelloAsso notification data into {Model}.", typeof(T).Name);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<Result<NotificationVerification>> VerifyAuthenticityAsync(HelloAssoNotification notification, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        // Payment / Order notifications reference a resource we can re-fetch to prove authenticity.
        string? relativePath = notification.EventType switch
        {
            NotificationEventType.Payment when TryGetId(notification.Data, out var paymentId) => $"payments/{paymentId}",
            NotificationEventType.Order when TryGetId(notification.Data, out var orderId) => $"orders/{orderId}",
            _ => null,
        };

        if (relativePath == null)
        {
            return Result<NotificationVerification>.Ok(new NotificationVerification
            {
                IsAuthentic = false,
                Reason = $"Authenticity verification is not supported for event type '{notification.EventType}' (or the payload had no id).",
            });
        }

        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<NotificationVerification>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/{relativePath}";
        var request = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config)
            .WithJsonAccept();

        var response = await Context.HttpClient.SendAsync(request, cancellationToken);
        var verification = new NotificationVerification
        {
            IsAuthentic = response.IsSuccessStatusCode,
            Reason = response.IsSuccessStatusCode ? null : $"Referenced resource '{relativePath}' could not be confirmed ({(int)response.StatusCode}).",
        };
        return Result<NotificationVerification>.Ok(verification);
    }

    private static bool TryGetId(JsonElement data, out long id)
    {
        id = 0;
        if (data.ValueKind == JsonValueKind.Object && data.TryGetProperty("id", out var idElement) && idElement.ValueKind == JsonValueKind.Number)
        {
            return idElement.TryGetInt64(out id);
        }
        return false;
    }
}
