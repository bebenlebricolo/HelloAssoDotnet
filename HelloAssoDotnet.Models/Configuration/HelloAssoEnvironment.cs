namespace HelloAssoDotnet.Models.Configuration;

/// <summary>
/// Selects which HelloAsso backend the client talks to.
/// <see aref="https://dev.helloasso.com/docs/environnements-production-et-sandbox-test"/>
/// </summary>
public enum HelloAssoEnvironment
{
    /// <summary>
    /// Live environment (real associations, real money).
    /// </summary>
    Production,

    /// <summary>
    /// Sandbox environment used to test an integration without impacting a real association.
    /// </summary>
    Sandbox
}

/// <summary>
/// Resolves the API and OAuth2 base URIs for a given <see cref="HelloAssoEnvironment"/>.
/// This keeps the environment switch in a single place instead of hard-coding URLs in the client.
/// </summary>
public static class HelloAssoEnvironmentUris
{
    /// <summary>
    /// Returns the versioned REST API base URI (the <c>/v5</c> root) for the environment.
    /// </summary>
    /// <param name="environment">Targeted environment</param>
    /// <returns>Base URI used to build API calls</returns>
    public static Uri GetApiBaseUri(HelloAssoEnvironment environment)
    {
        return environment switch
        {
            HelloAssoEnvironment.Sandbox => new Uri("https://api.helloasso-sandbox.com/v5"),
            _ => new Uri("https://api.helloasso.com/v5"),
        };
    }

    /// <summary>
    /// Returns the OAuth2 token endpoint for the environment.
    /// </summary>
    /// <param name="environment">Targeted environment</param>
    /// <returns>OAuth2 token endpoint</returns>
    public static Uri GetOauthTokenUri(HelloAssoEnvironment environment)
    {
        return environment switch
        {
            HelloAssoEnvironment.Sandbox => new Uri("https://api.helloasso-sandbox.com/oauth2/token"),
            _ => new Uri("https://api.helloasso.com/oauth2/token"),
        };
    }
}
