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
            // The webhook envelope is read by hand with JsonDocument instead of being deserialized into a
            // fixed type, because the body is polymorphic: the "data" property carries a different shape for
            // each "eventType", and that shape is not described by the OpenAPI spec. Here we only extract the
            // stable envelope fields (eventType + raw data + metadata); the payload interpretation is deferred
            // to ReadData, and the raw JsonElement is kept as a fallback for unknown/new event types.
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
        switch (notification.EventType)
        {
            case NotificationEventType.Payment:
                if (TryDeserialize<PaymentResponse>(notification.Data, out var payment))
                {
                    return new PaymentNotificationPayload(payment!);
                }
                return null;

            case NotificationEventType.Order:
                if (TryDeserialize<OrderDetails>(notification.Data, out var order))
                {
                    return new OrderNotificationPayload(order!);
                }
                return null;

            case NotificationEventType.Form:
                if (TryDeserialize<FormNotificationData>(notification.Data, out var form))
                {
                    return new FormNotificationPayload(form!);
                }
                return null;

            case NotificationEventType.Organization:
                if (TryDeserialize<OrganizationNotificationData>(notification.Data, out var organization))
                {
                    return new OrganizationNotificationPayload(organization!);
                }
                return null;

            default:
                return null;
        }
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
        string? relativePath = null;
        switch (notification.EventType)
        {
            case NotificationEventType.Payment:
                if (TryGetId(notification.Data, out var paymentId))
                {
                    relativePath = $"payments/{paymentId}";
                }
                break;

            case NotificationEventType.Order:
                if (TryGetId(notification.Data, out var orderId))
                {
                    relativePath = $"orders/{orderId}";
                }
                break;
        }

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
