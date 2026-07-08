namespace HelloAssoDotnet.Models.Api.Base;

/// <summary>
/// An image displayed on an organization/form public page.
/// <see aref="HelloAsso.Api.V5.Common.Models.Common.ImageModel"/>
/// </summary>
public record ImageModel
{
    /// <summary>
    /// Optional caption of the image.
    /// </summary>
    public string? Caption { get; set; }

    /// <summary>
    /// Stable identifier of the image.
    /// </summary>
    public int Id { get; set; } = 0;

    /// <summary>
    /// Public URL of the image.
    /// </summary>
    public string? Url { get; set; }
}

/// <summary>
/// A video displayed on an organization/form public page.
/// <see aref="HelloAsso.Api.V5.Common.Models.Common.VideoModel"/>
/// </summary>
public record VideoModel
{
    /// <summary>
    /// Stable identifier of the video.
    /// </summary>
    public int Id { get; set; } = 0;

    /// <summary>
    /// Public URL of the video.
    /// </summary>
    public string? Url { get; set; }
}
