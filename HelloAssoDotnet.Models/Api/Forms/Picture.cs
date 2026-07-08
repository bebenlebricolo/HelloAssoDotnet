namespace HelloAssoDotnet.Models.Api.Forms;

/// <summary>
/// Represents a picture record
/// </summary>
public record Picture
{
    /// <summary>
    /// Picture's Id
    /// </summary>
    public uint? Id { get; set; }

    /// <summary>
    /// Picture's FileName
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Picture's public Url
    /// </summary>
    public string? PublicUrl { get; set; }
}
