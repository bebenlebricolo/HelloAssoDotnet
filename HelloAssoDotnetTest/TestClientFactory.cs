using HelloAssoDotnet.Client;
using HelloAssoDotnet.Extensions;
using HelloAssoDotnet.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelloAssoDotnetTest;

/// <summary>
/// Builds a fully DI-wired <see cref="IHelloAssoClient"/> for tests. The mocked primary message handler is
/// plugged into the connection's named HttpClient, so the client is exercised through the same object graph
/// that production code gets from <c>AddHelloAsso</c>.
/// </summary>
internal static class TestClientFactory
{
    /// <summary>
    /// Builds the client, wiring the given handler, secrets service and configuration through DI.
    /// </summary>
    /// <param name="handler">Primary message handler backing the HttpClient (usually a Moq mock).</param>
    /// <param name="secrets">Secrets service supplying the client id / secret.</param>
    /// <param name="configuration">Configuration carrying the HelloAsso section.</param>
    /// <returns>The resolved <see cref="IHelloAssoClient"/>.</returns>
    public static IHelloAssoClient Build(HttpMessageHandler handler,
                                         IHelloAssoSecretsService secrets,
                                         IConfiguration configuration)
    {
        var services = new ServiceCollection();
        services.AddSingleton(configuration);
        services.AddSingleton(secrets);
        services.AddLogging();
        services.AddHelloAsso();
        services.AddHttpClient(HelloAssoConnection.HttpClientName)
                .ConfigurePrimaryHttpMessageHandler(() => handler);

        return services.BuildServiceProvider().GetRequiredService<IHelloAssoClient>();
    }
}
