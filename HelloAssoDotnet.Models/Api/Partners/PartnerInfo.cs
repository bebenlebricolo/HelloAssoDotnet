namespace HelloAssoDotnet.Models.Api.Partners;

/// <summary>
/// Information about the calling partner (API client behind the current credentials).
/// Mirrors the API "PartnerPublicModel" schema.
/// <see aref="https://dev.helloasso.com/reference/get_partners-me"/>
/// </summary>
public record PartnerInfo
{
    /// <summary>
    /// Partner name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Partner display name.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Partner description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Public URL of the partner.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Square logo URL of the partner.
    /// </summary>
    public string? Logo { get; set; }

    /// <summary>
    /// Rectangular logo URL of the partner.
    /// </summary>
    public string? LogoRectangle { get; set; }

    /// <summary>
    /// The API client (credentials) associated with the partner.
    /// </summary>
    public ApiClientModel? ApiClient { get; set; }

    /// <summary>
    /// Notification (webhook) URLs configured for the partner.
    /// </summary>
    public List<ApiUrlNotificationModel> UrlNotificationList { get; set; } = new List<ApiUrlNotificationModel>();

    /// <summary>
    /// Aggregated statistics for the partner.
    /// </summary>
    public PartnerStatisticsModel? PartnerStatistics { get; set; }
}

/// <summary>
/// The API client (credentials + privileges) of a partner.
/// Mirrors the API "ApiClientModel" schema.
/// </summary>
public record ApiClientModel
{
    /// <summary>
    /// Client identifier.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Client secret.
    /// </summary>
    public string? Secret { get; set; }

    /// <summary>
    /// Partner name owning the client.
    /// </summary>
    public string? PartnerName { get; set; }

    /// <summary>
    /// Privileges granted to the client.
    /// </summary>
    public List<string> Privileges { get; set; } = new List<string>();

    /// <summary>
    /// Domain registered for the client.
    /// </summary>
    public string? Domain { get; set; }
}

/// <summary>
/// A partner notification (webhook) URL together with its type and signature key.
/// Mirrors the API "ApiUrlNotificationModel" schema.
/// </summary>
public record ApiUrlNotificationModel
{
    /// <summary>
    /// The notification URL.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// The kind of notification pushed to this URL.
    /// </summary>
    public ApiNotificationType ApiNotificationType { get; set; }

    /// <summary>
    /// Signature key allowing verification of the authenticity of notifications.
    /// </summary>
    public string? SignatureKey { get; set; }
}

/// <summary>
/// Aggregated statistics for a partner. Mirrors the API "PartnerStatisticsModel" schema.
/// </summary>
public record PartnerStatisticsModel
{
    /// <summary>
    /// Number of organizations linked to the partner.
    /// </summary>
    public int LinkedOrganizationsCount { get; set; } = 0;

    /// <summary>
    /// Total amount collected by linked organizations, in cents.
    /// </summary>
    public long LinkedOrganizationsCollectedAmount { get; set; } = 0;

    /// <summary>
    /// Total amount collected through checkout, in cents.
    /// </summary>
    public long CheckoutCollectedAmount { get; set; } = 0;

    /// <summary>
    /// Number of organizations using checkout.
    /// </summary>
    public int OrganizationsUsingCheckout { get; set; } = 0;

    /// <summary>
    /// Number of organizations for which an access token is available.
    /// </summary>
    public int AvailableOrganizationsAccessTokenCount { get; set; } = 0;
}

/// <summary>
/// The kind of notification pushed to a partner notification URL.
/// Mirrors the API "ApiNotificationType" schema.
/// </summary>
public enum ApiNotificationType
{
    /// <summary>
    /// Payment notifications.
    /// </summary>
    Payment,

    /// <summary>
    /// Order notifications.
    /// </summary>
    Order,

    /// <summary>
    /// Form notifications.
    /// </summary>
    Form,

    /// <summary>
    /// Organization notifications.
    /// </summary>
    Organization
}
