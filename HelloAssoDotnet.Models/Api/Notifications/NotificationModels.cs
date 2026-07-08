using System.Text.Json;
using HelloAssoDotnet.Models.Api.Base;
using HelloAssoDotnet.Models.Api.Forms;
using HelloAssoDotnet.Models.Api.Order;
using HelloAssoDotnet.Models.PublicApi;

namespace HelloAssoDotnet.Models.Api.Notifications;

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

// -----------------------------------------------------------------------------------------------------
// Known, typed notification payloads.
//
// HelloAsso webhook bodies are polymorphic: the shape of the "data" object is decided at runtime by the
// "eventType" field. This payload is NOT part of the OpenAPI specification, so the records below are
// hand-written from HelloAsso's documented notification examples
// (https://dev.helloasso.com/docs/notification-exemple).
//
// The parser (NotificationsClient) branches on "eventType" with a plain switch and deserializes "data"
// into the matching record here. We deliberately avoid System.Text.Json polymorphic attributes
// ([JsonPolymorphic]/[JsonDerivedType]) and any reflection-based dispatch: the mapping must stay fully
// explicit and readable in source. The raw JsonElement (HelloAssoNotification.Data) is always kept as a
// fallback, so an unknown or brand-new event type never throws - the caller can still read it by hand.
// -----------------------------------------------------------------------------------------------------

/// <summary>
/// Base type of the strongly-typed payload obtained from a <see cref="HelloAssoNotification"/>.
/// Match on the concrete subtype to read the typed data, e.g.
/// <c>if (payload is PaymentNotificationPayload p) { ... p.Payment ... }</c>.
/// </summary>
public abstract record NotificationPayload;

/// <summary>
/// Typed payload of a <see cref="NotificationEventType.Payment"/> notification.
/// </summary>
/// <param name="Payment">The payment carried by the notification.</param>
public sealed record PaymentNotificationPayload(PaymentResponse Payment) : NotificationPayload;

/// <summary>
/// Typed payload of an <see cref="NotificationEventType.Order"/> notification.
/// </summary>
/// <param name="Order">The order carried by the notification.</param>
public sealed record OrderNotificationPayload(OrderDetails Order) : NotificationPayload;

/// <summary>
/// Typed payload of a <see cref="NotificationEventType.Form"/> notification.
/// </summary>
/// <param name="Form">The form descriptor carried by the notification.</param>
public sealed record FormNotificationPayload(FormNotificationData Form) : NotificationPayload;

/// <summary>
/// Typed payload of an <see cref="NotificationEventType.Organization"/> notification.
/// </summary>
/// <param name="Organization">The organization descriptor carried by the notification.</param>
public sealed record OrganizationNotificationPayload(OrganizationNotificationData Organization) : NotificationPayload;

/// <summary>
/// Data carried by a form notification. Modeled from the documented notification examples; the raw
/// <see cref="HelloAssoNotification.Data"/> stays authoritative for any field not represented here.
/// </summary>
public record FormNotificationData
{
    /// <summary>
    /// Type of the form.
    /// </summary>
    public FormType FormType { get; set; } = FormType.Event;

    /// <summary>
    /// Form slug (lowercase, kebab-cased name used across the API).
    /// </summary>
    public string? FormSlug { get; set; }

    /// <summary>
    /// Form display name.
    /// </summary>
    public string? FormName { get; set; }

    /// <summary>
    /// Slug of the organization owning the form.
    /// </summary>
    public string? OrganizationSlug { get; set; }

    /// <summary>
    /// Name of the organization owning the form.
    /// </summary>
    public string? OrganizationName { get; set; }
}

/// <summary>
/// Data carried by an organization notification. Modeled from the documented notification examples; the raw
/// <see cref="HelloAssoNotification.Data"/> stays authoritative for any field not represented here.
/// </summary>
public record OrganizationNotificationData
{
    /// <summary>
    /// Organization display name.
    /// </summary>
    public string? OrganizationName { get; set; }

    /// <summary>
    /// Organization slug (lowercase, kebab-cased name used across the API).
    /// </summary>
    public string? OrganizationSlug { get; set; }

    /// <summary>
    /// Legal type of the organization.
    /// </summary>
    public OrganizationType OrganizationType { get; set; } = OrganizationType.Association1901;
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
