namespace HelloAssoDotnet.Models.HelloAssoApi.Auth;

/// <summary>
/// HelloAsso authentication tokens (cached for later reuse)
/// </summary>
public record AuthTokens
{
    /// <summary>
    /// Jwt access token
    /// </summary>
    public string AccessToken { get; set; } = "";

    /// <summary>
    /// Expiration, in seconds (30 minutes, by default)
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Jwt RefreshToken
    /// </summary>
    public string RefreshToken { get; set; } = "";

    /// <summary>
    /// Token type (usually "bearer")
    /// </summary>
    public string TokenType { get; set; } = "";
}
