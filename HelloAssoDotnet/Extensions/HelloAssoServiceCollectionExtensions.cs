using HelloAssoDotnet.Client;
using HelloAssoDotnet.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HelloAssoDotnet.Extensions;

/// <summary>
/// Dependency-injection helpers to register the HelloAsso client and its dependencies.
/// </summary>
public static class HelloAssoServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IHelloAssoClient"/> as a typed <see cref="System.Net.Http.HttpClient"/>.
    /// The secrets service (<see cref="IHelloAssoSecretsService"/>), logger and configuration are resolved
    /// from the container, so make sure an <see cref="IHelloAssoSecretsService"/> is registered (either
    /// manually or via the overload taking a secrets file path).
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>The <see cref="IHttpClientBuilder"/> so the HttpClient can be further configured.</returns>
    public static IHttpClientBuilder AddHelloAsso(this IServiceCollection services)
    {
        return services.AddHttpClient<IHelloAssoClient, HelloAssoClient>();
    }

    /// <summary>
    /// Registers a default <see cref="IHelloAssoSecretsService"/> (reading the given secrets file, then falling
    /// back to environment variables) and <see cref="IHelloAssoClient"/> as a typed HttpClient.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="secretsFilePath">Path to the secrets file (may be null to only use environment variables).</param>
    /// <returns>The <see cref="IHttpClientBuilder"/> so the HttpClient can be further configured.</returns>
    public static IHttpClientBuilder AddHelloAsso(this IServiceCollection services, string? secretsFilePath)
    {
        services.TryAddSingleton<IHelloAssoSecretsService>(_ => new DefaultHelloAssoSecretsService
        {
            SecretsFilePath = secretsFilePath,
        });
        return services.AddHelloAsso();
    }
}
