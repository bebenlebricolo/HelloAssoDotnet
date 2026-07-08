using HelloAssoDotnet.Client;
using HelloAssoDotnet.Extensions;
using HelloAssoDotnet.Models.Configuration;
using HelloAssoDotnet.Models.PublicApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// This sample shows how to configure and use the HelloAssoDotnet client from a
// plain console application, using the standard .NET generic host and DI.
//
// Credentials are NEVER committed. Provide them at runtime either via:
//   * environment variables HELLO_ASSO_CLIENT_ID / HELLO_ASSO_CLIENT_SECRET, or
//   * a secrets file pointed to by the "HelloAsso:SecretsFile" configuration key.
// See the README next to this file for details.

var builder = Host.CreateApplicationBuilder(args);

// Read the appsettings-bound configuration (SecretsFile + OrganizationSlug + Environment).
var helloAssoConfig = new AppsettingsConfiguration();
helloAssoConfig.FromConfig(builder.Configuration);

// Register the default secrets service (secrets file, then env-var fallback) and the client as a typed
// HttpClient in a single call. The logger and configuration are resolved from DI.
builder.Services.AddHelloAsso(helloAssoConfig.SecretsFile);

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
var client = host.Services.GetRequiredService<IHelloAssoClient>();

// 1. Authenticate (creates and caches the JWT tokens from the ClientId / ClientSecret).
//    Once authenticated, sub-client calls reuse the cached token automatically (no need to pass it around).
var authResult = await client.AuthenticateAsync();
if (!authResult.IsOk)
{
    logger.LogError("Authentication failed: {error}. Did you provide valid credentials?", authResult.Error);
    return 1;
}

var tokens = authResult.Value!;
logger.LogInformation("Authenticated successfully (token expires in {expiresIn}s).", tokens.ExpiresIn);

// 2. List the organization forms (the cached token is used under the hood).
var formsResult = await client.Forms.ListAsync(new ListOrganizationFormsRequest());
if (!formsResult.IsOk)
{
    logger.LogError("Failed to list organization forms: {error}", formsResult.Error);
    return 1;
}

var forms = formsResult.Value!;
Console.WriteLine($"Found {forms.Data.Count} form(s) for organization '{helloAssoConfig.OrganizationSlug}':");
foreach (var form in forms.Data)
{
    Console.WriteLine($"  - {form.Title} ({form.FormSlug})");
}

// 3. Optionally look up a payer's payments when an email is passed as the first argument.
var email = args.FirstOrDefault();
if (!string.IsNullOrWhiteSpace(email))
{
    var paymentsResult = await client.Payments.SearchForUserAsync(email);
    if (!paymentsResult.IsOk)
    {
        logger.LogError("Failed to get payments for '{email}': {error}", email, paymentsResult.Error);
        return 1;
    }

    var payments = paymentsResult.Value!;
    Console.WriteLine($"Found {payments.Data.Count} payment(s) for '{email}'.");
}

return 0;
