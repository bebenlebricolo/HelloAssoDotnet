namespace HelloAssoDotnet.Services;

/// <summary>
/// Very simple interface used to retrieve secrets either from Env or file system.
/// Can easily be mocked for testing purposes
/// </summary>
public interface IHelloAssoSecretsService
{
    /// <summary>
    /// Retrieves ClientId
    /// </summary>
    /// <returns></returns>
    public string? GetClientId();

    /// <summary>
    /// Retrieves Client Secret
    /// </summary>
    /// <returns></returns>
    public string? GetClientSecret();
}
