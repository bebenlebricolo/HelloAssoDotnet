using HelloAssoDotnet.Models.Utils;
using Microsoft.Extensions.Configuration;

namespace HelloAssoDotnet.Models.Configuration;

/// <summary>
/// Basic HelloAsso configuration
/// </summary>
public record AppsettingsConfiguration
{
    /// <summary>
    /// Name of the section in the configuration file.
    /// </summary>
    public static string SectionName = "HelloAsso";

    /// <summary>
    /// Pointer to SecretsFile
    /// </summary>
    public string SecretsFile { get; set; } = "";

    /// <summary>
    /// Organization slug used by HelloAsso.
    /// <example>my org name = "My Super Association" ; my org slug = "my-super-association"</example>
    /// </summary>
    public string OrganizationSlug { get; set; } = "";

    /// <summary>
    /// Selects the HelloAsso backend (Production or Sandbox).
    /// Defaults to <see cref="HelloAssoEnvironment.Production"/> when the key is missing or unparseable.
    /// </summary>
    public HelloAssoEnvironment Environment { get; set; } = HelloAssoEnvironment.Production;

    /// <summary>
    /// Versioned REST API base URL (the <c>/v5</c> root). Defaults to the URL matching <see cref="Environment"/>
    /// (production) and can be overridden from configuration (useful for sandbox or a custom gateway).
    /// </summary>
    public string ApiBaseUrl { get; set; } = DefaultApiBaseUrl(HelloAssoEnvironment.Production);

    /// <summary>
    /// OAuth2 token endpoint. Defaults to the URL matching <see cref="Environment"/> (production) and can be
    /// overridden from configuration.
    /// </summary>
    public string OauthTokenUrl { get; set; } = DefaultOauthTokenUrl(HelloAssoEnvironment.Production);

    /// <summary>
    /// Instance of the configuration
    /// </summary>
    /// <param name="section"></param>
    /// <returns></returns>
    public bool FromConfigSection(IConfigurationSection section)
    {
        bool success = true;
        var secretsFile = section.GetValue<string?>(nameof(SecretsFile));
        var organizationSlug = section.GetValue<string?>(nameof(OrganizationSlug));

        success &= secretsFile != null;
        success &= organizationSlug != null;

        SecretsFile = secretsFile ?? SecretsFile;
        OrganizationSlug = organizationSlug ?? OrganizationSlug;
        SecretsFile = EnvVarResolver.SusbtituteEnvInString(SecretsFile);

        // Environment is optional and defaults to Production. An unknown value is ignored (kept as-is).
        var environment = section.GetValue<string?>(nameof(Environment));
        if (!string.IsNullOrWhiteSpace(environment) && Enum.TryParse<HelloAssoEnvironment>(environment, ignoreCase: true, out var parsedEnvironment))
        {
            Environment = parsedEnvironment;
        }

        // URLs are resolved from the selected environment by default, and can be explicitly overridden by
        // providing the matching keys in configuration.
        var apiBaseUrl = section.GetValue<string?>(nameof(ApiBaseUrl));
        var oauthTokenUrl = section.GetValue<string?>(nameof(OauthTokenUrl));
        ApiBaseUrl = string.IsNullOrWhiteSpace(apiBaseUrl) ? DefaultApiBaseUrl(Environment) : apiBaseUrl;
        OauthTokenUrl = string.IsNullOrWhiteSpace(oauthTokenUrl) ? DefaultOauthTokenUrl(Environment) : oauthTokenUrl;

        return success;
    }

    /// <summary>
    /// Default REST API base URL for the given environment.
    /// </summary>
    /// <param name="environment">Targeted environment</param>
    /// <returns>Base URL used to build API calls</returns>
    public static string DefaultApiBaseUrl(HelloAssoEnvironment environment)
    {
        switch (environment)
        {
            case HelloAssoEnvironment.Sandbox:
                return "https://api.helloasso-sandbox.com/v5";
            default:
                return "https://api.helloasso.com/v5";
        }
    }

    /// <summary>
    /// Default OAuth2 token endpoint for the given environment.
    /// </summary>
    /// <param name="environment">Targeted environment</param>
    /// <returns>OAuth2 token endpoint</returns>
    public static string DefaultOauthTokenUrl(HelloAssoEnvironment environment)
    {
        switch (environment)
        {
            case HelloAssoEnvironment.Sandbox:
                return "https://api.helloasso-sandbox.com/oauth2/token";
            default:
                return "https://api.helloasso.com/oauth2/token";
        }
    }

    /// <summary>
    /// Tries to read from the configuration file itself
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public bool FromConfig(IConfiguration config)
    {
        var section = config.GetSection(SectionName);
        if (!section.Exists())
        {
            return false;
        }
        return FromConfigSection(section);
    }
}
