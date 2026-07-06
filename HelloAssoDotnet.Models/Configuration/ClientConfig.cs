namespace HelloAssoDotnet.Models.Configuration;

/// <summary>
/// Client configuration object is provided to clients upon instantiation.
/// It is used to retrieve data which is provided by the upper layer (the code that uses this client)
/// </summary>
public record ClientConfig
{
    /// <summary>
    /// User agent property used with requests
    /// </summary>
    public string UserAgent { get; set; } = "";

    /// <summary>
    /// Version of the User Agent
    /// </summary>
    public string UserAgentVersion { get; set; } = "";

    /// <summary>
    /// Readonly, full user agent string, used in Headers as-is.
    /// </summary>
    public string FullUserAgent => $"{UserAgent}/{UserAgentVersion}";
}
