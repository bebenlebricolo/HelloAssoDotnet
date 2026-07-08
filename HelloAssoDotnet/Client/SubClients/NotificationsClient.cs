using System.Text.Json;
using HelloAssoDotnet.Models.HelloAssoApi.Auth;
using HelloAssoDotnet.Models.HelloAssoApi.Notifications;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Utils;
using Microsoft.Extensions.Logging;

namespace HelloAssoDotnet.Client.SubClients;

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
