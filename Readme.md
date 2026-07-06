# HelloAsso Dotnet Client library

[![.NET Build and Test](https://github.com/bebenlebricolo/HelloAssoDotnet/actions/workflows/dotnet.yml/badge.svg)](https://github.com/bebenlebricolo/HelloAssoDotnet/actions/workflows/dotnet.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

This C# library (.net 10.0) provides a small client library that'll connect to HelloAsso api endpoints using an Api Key as its main authentication means.
It is Dependency Injection ready and follows common .Net MVC patterns, and comes with its own Test harness.
It's not meant to be fully exhaustive (and probably won't be by my sole action, I don't need to map 100% of the HelloAsso API for my needs).
But it'll serve as a good base for anyone, assuming someone wants to take over and implement other endpoints mapping.

For now, it mostly ships capabilities to :
* Authenticate (create JWT token)
* Refresh Token
* Lists user payments
* Get a single payment details

# Installation
```bash
dotnet add package HelloAssoDotnet
```
The public data models are also published as a standalone package (`HelloAssoDotnet.Models`) for consumers that only need the models.

# Quickstart
A complete, runnable example lives in [`samples/HelloAssoDotnet.Sample.Console`](samples/HelloAssoDotnet.Sample.Console/README.md).
It shows how to wire the client through the .NET generic host, authenticate, and pull real data from your organization.

# Endpoints
See [`docs/Endpoints.md`](docs/Endpoints.md) for the list of HelloAsso API endpoints currently mapped by the client.

> Note : for now, our JWT token solely provided by the API key alone <u>***doesn't have***</u> sufficient permissions to be able to download a Pdf Receipt from the payment's PaymentReceiptUrl.
This might become available in the future when fine-grained rights can be implemented by HelloAsso infrastructure.
But the client still exposes the calls, so that in the future this might be enabled more easily.

# Instantiation
In your `Program.cs`, simply insert :
```Cs
private static DefaultHelloAssoSecretsService CreateHelloAssoDefaultSecretsService(WebApplicationBuilder builder)
{
    var configuration = builder.Configuration;
    var helloAssoConfig = new AppsettingsConfiguration();
    var success = helloAssoConfig.FromConfig(configuration);

    var service = new DefaultHelloAssoSecretsService();
    if (success)
    {
        service.SecretsFilePath = helloAssoConfig.SecretsFile;
    }

    return service;
}

// ...
public static void Main(string[] args)
{
    // Register services
    builder.Services.AddScoped<IHelloAssoClient, HelloAssoClient>();
    builder.Services.AddSingleton<IHelloAssoSecretsService>(CreateHelloAssoDefaultSecretsService(builder));
    // ...
}
```

# Usage
This lib's usage is fairly straight forward :

```Cs
/// <summary>
/// Hello asso service (client) implementation
/// </summary>
public class HelloAssoService : IHelloAssoService
{
    // Private ref to the client, injected on the Constructor invocation
    private readonly IHelloAssoClient _client;

    //...
    public async Task<Result> DoSomething()
    {
        // Note, this client library might exceptionally throw, however it's main error communication protocol still resides in the Result<T> pattern object.
        var refreshResult = await _client.RefreshTokenAsync(oldTokens);
        if (!refreshResult.IsOk)
        {
            _logger.LogError("Refresh token failed.");
            return null;
        }

        var payload = refreshResult.Value!;
        // ...
    }
}
```

# Configuration
This library requires a `ClientId` and a `ClientSecret` (as per retrieved on HelloAsso dashboard, using an admin account).
Those can be provided using 2 methods :
* Either use a secret file (for instance, a volume mapped to the container while running)
* Environment variables.

## Configuration types
The library intentionally separates two distinct configuration concerns:
* `AppsettingsConfiguration` (`HelloAssoDotnet.Models.Configuration`): the **configuration-bound** settings (`SecretsFile`, `OrganizationSlug`). As its name suggests, it is meant to be pulled from your `appsettings.json` (via `FromConfig` / `FromConfigSection`) and supports environment-variable substitution.
* `ClientConfig` (`HelloAssoDotnet.Models.Configuration`): a **programmatic**, plain data structure passed at client instantiation to supply values that only the calling layer knows (e.g. `UserAgent` / `UserAgentVersion`).

## Secret file structure
This file can be stored somewhere on the disk.
```json
{
  "clientId" : "1234567890°+",
  "clientSecret" : "Some hashed secret"
}
```

## Appsettings sections
It is also fairly straight forward to use the `appsettings.json` files to configure the project :
```json
{
  // Note that Environment variables substitution is available in HelloAssoDotnet.Models/Configuration/AppsettingsConfiguration.cs
  "HelloAsso": {
    "SecretsFile": "$HOME/.config/MyApp/.secrets/helloasso-secrets.json",
    "OrganizationSlug": "my-organization"
  }
}
```
