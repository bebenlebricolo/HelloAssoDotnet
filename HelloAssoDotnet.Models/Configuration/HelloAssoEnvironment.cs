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
