# HelloAsso Dotnet Client library

[![.NET Build and Test](https://github.com/bebenlebricolo/HelloAssoDotnet/actions/workflows/dotnet.yml/badge.svg)](https://github.com/bebenlebricolo/HelloAssoDotnet/actions/workflows/dotnet.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

This C# library (.net 10.0) provides a small client library that'll connect to HelloAsso api endpoints using an Api Key as its main authentication means.
It is Dependency Injection ready and follows common .Net MVC patterns, and comes with its own Test harness.
It's not meant to be fully exhaustive (and probably won't be by my sole action, I don't need to map 100% of the HelloAsso API for my needs).
But it'll serve as a good base for anyone, assuming someone wants to take over and implement other endpoints mapping.

The API surface is grouped into **resource sub-clients** accessed from the root client
(e.g. `client.Orders.GetAsync(...)`, `client.Payments.SearchAsync(...)`). It ships:
* Authentication (create JWT token) with **in-memory token caching + auto-refresh**
* Refresh Token
* Organizations, Forms (list / public details / form types / items / orders / payments / stats)
* Orders (by id, organization listing, event tickets PDF)
* Items, Payments (search + filters, by id, receipt PDF)
* Checkout (read intent), Directory searches, Partners / Users, reference Values, CashOut export
* Notifications (webhook) parsing + authenticity verification
* Pagination auto-pagers (`IAsyncEnumerable<>`), a Production/Sandbox switch and `CancellationToken` support throughout

> **Breaking change (v2.0.0):** the flat methods on `IHelloAssoClient` (e.g. `GetPaymentForUserAsync`,
> `GetFormsFromOrganization`, `GetFormDetailsAsync`, `GetOrderDetailsAsync`, `GetPaymentReceiptPdfAsync`,
> `GetEventTicketPdf`) were moved onto sub-clients. There are no `[Obsolete]` shims - this is a hard cutover.
> Map old calls like so: `client.Payments.SearchForUserAsync(email)`, `client.Forms.ListAsync(request)`,
> `client.Forms.GetPublicDetailsAsync(slug, type)`, `client.Orders.GetAsync(id)`,
> `client.Payments.GetReceiptPdfAsync(payment)`, `client.Orders.GetEventTicketsPdfAsync(payment)`.
> Explicit `AuthTokens` are now optional on every method (omit them to use the cached token).

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
In your `Program.cs`, register the client with the `AddHelloAsso` DI extension. It registers a default
secrets service (reading the given secrets file, then falling back to environment variables) and the client
as a typed `HttpClient` :
```Cs
using HelloAssoDotnet.Extensions;

// ...
var helloAssoConfig = new AppsettingsConfiguration();
helloAssoConfig.FromConfig(builder.Configuration);

builder.Services.AddHelloAsso(helloAssoConfig.SecretsFile);
```

If you prefer to register the secrets service yourself, use the parameterless overload:
```Cs
builder.Services.AddSingleton<IHelloAssoSecretsService>(/* your own */);
builder.Services.AddHelloAsso();
```

# Usage
The root client owns authentication and caches the token, so once you have authenticated the sub-clients
reuse it automatically (you can still pass explicit `AuthTokens` to any method to manage tokens yourself) :

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
        // Authenticate once; the returned token is cached and auto-refreshed by the root client.
        var authResult = await _client.AuthenticateAsync();
        if (!authResult.IsOk)
        {
            _logger.LogError("Authentication failed.");
            return null;
        }

        // Sub-client calls use the cached token (no need to pass it around).
        var payments = await _client.Payments.SearchForUserAsync("someone@somewhere.com");
        if (!payments.IsOk)
        {
            _logger.LogError("Payment search failed.");
            return null;
        }

        // Listing endpoints also expose an auto-pager that follows the continuation token:
        await foreach (var form in _client.Forms.ListAllAsync(new ListOrganizationFormsRequest()))
        {
            // ...
        }
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
* `AppsettingsConfiguration` (`HelloAssoDotnet.Models.Configuration`): the **configuration-bound** settings (`SecretsFile`, `OrganizationSlug`, `Environment`). As its name suggests, it is meant to be pulled from your `appsettings.json` (via `FromConfig` / `FromConfigSection`) and supports environment-variable substitution. `Environment` is optional (`Production` by default) and can be set to `Sandbox` to target the HelloAsso sandbox host.
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
    "OrganizationSlug": "my-organization",
    "Environment": "Production"
  }
}
```
