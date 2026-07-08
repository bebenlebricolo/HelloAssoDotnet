using System.Text.Json;
using HelloAssoDotnet.Client;
using HelloAssoDotnet.Models.Secrets;
using HelloAssoDotnet.Utils;

namespace HelloAssoDotnet.Services;

/// <summary>
/// Default implementation for Hello asso secrets retriever. Uses environment variables by default.
/// </summary>
public class DefaultHelloAssoSecretsService : IHelloAssoSecretsService
{
    /// <summary>
    /// Filepath of secrets file
    /// </summary>
    public string? SecretsFilePath { get; set; }

    /// <summary>
    /// Reads content from secrets file and return it.
    /// </summary>
    /// <param name="secretsFilePath"></param>
    /// <returns></returns>
    public static SecretsFileModel? ReadFromFile(string secretsFilePath)
    {
        using FileStream fileStream = new(secretsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var model = JsonSerializer.Deserialize<SecretsFileModel>(fileStream, JsonOptionsProvider.GetJsonOptions());
        return model;
    }

    /// <summary>
    /// Retrieves Client id from environment
    /// </summary>
    /// <returns></returns>
    public string? GetClientId()
    {
        if (File.Exists(SecretsFilePath))
        {
            var model = ReadFromFile(SecretsFilePath);
            return model?.ClientId;
        }

        return Environment.GetEnvironmentVariable("HELLO_ASSO_CLIENT_ID");
    }

    /// <summary>
    /// Retrieves Client secret from environment
    /// </summary>
    /// <returns></returns>
    public string? GetClientSecret()
    {
        if (File.Exists(SecretsFilePath))
        {
            var model = ReadFromFile(SecretsFilePath);
            return model?.ClientSecret;
        }

        return Environment.GetEnvironmentVariable("HELLO_ASSO_CLIENT_SECRET");
    }
}
