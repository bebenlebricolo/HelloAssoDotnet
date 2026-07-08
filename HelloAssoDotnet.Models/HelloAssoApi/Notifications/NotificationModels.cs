using System.Text.Json;

namespace HelloAssoDotnet.Models.HelloAssoApi.Notifications;

/// <summary>
/// Type of event carried by a HelloAsso notification (webhook).
/// <see aref="https://dev.helloasso.com/docs/notification-exemple"/>
/// </summary>
public enum NotificationEventType
{
    /// <summary>
    /// The notification could not be mapped to a known event type.
    /// </summary>
    Unknown,

    /// <summary>
    /// A payment event (authorized, refunded, refused, contested, ...).
    /// </summary>
    Payment,

    /// <summary>
    /// An order event (order created).
    /// </summary>
    Order,

    /// <summary>
    /// A form event (form created).
    /// </summary>
    Form,

    /// <summary>
    /// An organization event (name change, payment eligibility change, ...).
    /// </summary>
    Organization
}

/// <summary>
/// A parsed HelloAsso notification. The <see cref="Data"/> payload shape depends on <see cref="EventType"/>;
/// it is kept as a raw <see cref="JsonElement"/> so callers can deserialize it into the model they need.
/// </summary>
public record HelloAssoNotification
{
    /// <summary>
    /// The kind of event carried by this notification.
    /// </summary>
    public NotificationEventType EventType { get; set; } = NotificationEventType.Unknown;

    /// <summary>
    /// Raw event payload. Its shape depends on <see cref="EventType"/>.
    /// </summary>
    public JsonElement Data { get; set; }

    /// <summary>
    /// Optional free-form metadata that was attached to the originating action (e.g. a checkout intent).
    /// </summary>
    public JsonElement? Metadata { get; set; }
}

/// <summary>
/// Result of a notification authenticity verification.
/// </summary>
public record NotificationVerification
{
    /// <summary>
    /// Whether the notification was successfully verified as authentic.
    /// </summary>
    public bool IsAuthentic { get; init; }

    /// <summary>
    /// Optional human-readable reason (useful when the verification failed).
    /// </summary>
    public string? Reason { get; init; }
}
