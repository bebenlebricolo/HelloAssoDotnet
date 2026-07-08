namespace HelloAssoDotnet.Models.Secrets;

/// <summary>
/// Represents a very simple secrets json file model.
/// </summary>
public record SecretsFileModel
{
    /// <summary>
    /// Client id
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Client secret
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;
}
