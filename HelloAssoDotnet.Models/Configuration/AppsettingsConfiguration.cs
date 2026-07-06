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

        return success;
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
