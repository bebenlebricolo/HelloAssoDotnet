namespace HelloAssoDotnet.Models.HelloAssoApi.Partners;

/// <summary>
/// Information about the calling partner (API client behind the current credentials).
/// <see aref="https://dev.helloasso.com/reference/get_partners-me"/>
/// </summary>
public record PartnerInfo
{
    /// <summary>
    /// Partner display name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Domain registered for the partner API client.
    /// </summary>
    public string? Domain { get; set; }

    /// <summary>
    /// Public URL of the partner.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Main notification (webhook) URL configured for the partner, if any.
    /// </summary>
    public string? NotificationUrl { get; set; }

    /// <summary>
    /// Privileges granted to the partner clientId.
    /// </summary>
    public List<string> Privileges { get; set; } = new List<string>();
}
