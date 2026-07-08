using HelloAssoDotnet.Client;
using HelloAssoDotnet.Models.Configuration;
using HelloAssoDotnet.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HelloAssoDotnet.Extensions;

/// <summary>
/// Dependency-injection helpers to register the HelloAsso client and its dependencies.
/// </summary>
public static class HelloAssoServiceCollectionExtensions
{
    /// <summary>
    /// Registers the full HelloAsso object graph: the shared <see cref="HelloAssoConnection"/> (and its named
    /// <see cref="System.Net.Http.HttpClient"/>), the resource sub-clients and the <see cref="IHelloAssoClient"/>
    /// facade. The secrets service (<see cref="IHelloAssoSecretsService"/>), logger and configuration are resolved
    /// from the container, so make sure an <see cref="IHelloAssoSecretsService"/> is registered (either manually
    /// or via the overload taking a secrets file path) and that an <see cref="IConfiguration"/> is available.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddHelloAsso(this IServiceCollection services)
    {
        // Named HttpClient owned by the connection.
        services.AddHttpClient(HelloAssoConnection.HttpClientName);

        // Static configuration, built once from IConfiguration.
        services.TryAddSingleton(sp =>
        {
            var configuration = new AppsettingsConfiguration();
            configuration.FromConfig(sp.GetRequiredService<IConfiguration>());
            return configuration;
        });

        // Shared connection (owns the token cache) - a single instance exposed both concretely and via the
        // sub-client context interface.
        services.TryAddSingleton<HelloAssoConnection>();
        services.TryAddSingleton<IHelloAssoClientContext>(sp => sp.GetRequiredService<HelloAssoConnection>());

        // Resource sub-clients.
        services.TryAddSingleton<IOrganizationsClient, OrganizationsClient>();
        services.TryAddSingleton<IFormsClient, FormsClient>();
        services.TryAddSingleton<IOrdersClient, OrdersClient>();
        services.TryAddSingleton<IItemsClient, ItemsClient>();
        services.TryAddSingleton<IPaymentsClient, PaymentsClient>();
        services.TryAddSingleton<ICheckoutClient, CheckoutClient>();
        services.TryAddSingleton<IDirectoryClient, DirectoryClient>();
        services.TryAddSingleton<IPartnersClient, PartnersClient>();
        services.TryAddSingleton<IUsersClient, UsersClient>();
        services.TryAddSingleton<IValuesClient, ValuesClient>();
        services.TryAddSingleton<ICashOutClient, CashOutClient>();
        services.TryAddSingleton<INotificationsClient, NotificationsClient>();

        // Facade.
        services.TryAddSingleton<IHelloAssoClient, HelloAssoClient>();

        return services;
    }

    /// <summary>
    /// Registers a default <see cref="IHelloAssoSecretsService"/> (reading the given secrets file, then falling
    /// back to environment variables) and the full HelloAsso object graph.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="secretsFilePath">Path to the secrets file (may be null to only use environment variables).</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddHelloAsso(this IServiceCollection services, string? secretsFilePath)
    {
        services.TryAddSingleton<IHelloAssoSecretsService>(_ => new DefaultHelloAssoSecretsService
        {
            SecretsFilePath = secretsFilePath,
        });
        return services.AddHelloAsso();
    }
}
