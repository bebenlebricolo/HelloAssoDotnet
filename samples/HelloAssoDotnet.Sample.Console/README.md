# HelloAssoDotnet console sample

A minimal console application showing how to configure and use the
`HelloAssoDotnet` client with the standard .NET generic host and dependency
injection.

It:

1. Authenticates against the HelloAsso API.
2. Lists the forms of your organization.
3. Optionally lists a payer's payments when you pass their email as an argument.

## Configuration

Two things are needed:

- Your organization slug, set in [`appsettings.json`](appsettings.json) under
  `HelloAsso:OrganizationSlug`.
- Your API credentials (`ClientId` / `ClientSecret`), which are **never**
  committed. Provide them in one of two ways.

### Option A - environment variables (simplest)

Leave `HelloAsso:SecretsFile` empty (or unset) and export:

```bash
export HELLO_ASSO_CLIENT_ID="your-client-id"
export HELLO_ASSO_CLIENT_SECRET="your-client-secret"
```

### Option B - secrets file

Create a JSON file somewhere outside the repository:

```json
{
  "clientId": "your-client-id",
  "clientSecret": "your-client-secret"
}
```

Then point to it, either directly in `appsettings.json` or via an environment
variable that the configuration substitutes at runtime:

```bash
export HELLO_ASSO_SECRETS_FILE="/path/to/helloasso-secrets.json"
```

(The `HelloAsso:SecretsFile` value in `appsettings.json` defaults to
`$HELLO_ASSO_SECRETS_FILE`, which is resolved through environment-variable
substitution.)

## Run

```bash
# List the organization forms
dotnet run --project samples/HelloAssoDotnet.Sample.Console

# Also list payments for a given payer email
dotnet run --project samples/HelloAssoDotnet.Sample.Console -- someone@example.com
```

Where you obtain credentials: the `ClientId` and `ClientSecret` come from the
HelloAsso dashboard using an admin account.
