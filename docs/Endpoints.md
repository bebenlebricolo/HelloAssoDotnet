# Mapped HelloAsso endpoints

This is the set of HelloAsso REST API endpoints mapped by
[`HelloAssoClient`](../HelloAssoDotnet/Client/HelloAssoClient.cs) and its resource
sub-clients. It is not an exhaustive mapping of the HelloAsso API - only what the
library exposes today (read-only surface; write operations are planned, see the
[SDK Expansion Roadmap](SdkExpansionRoadmap.md)).

- API base URL: `https://api.helloasso.com/v5` (production) or
  `https://api.helloasso-sandbox.com/v5` (sandbox), selected by
  `HelloAsso:Environment`.
- OAuth base URL: `.../oauth2/token` on the same host.
- `{orgSlug}` is taken from configuration (`HelloAsso:OrganizationSlug`).

Every sub-client method accepts an optional `AuthTokens` (to bypass the cached
token) and a `CancellationToken`. Listing methods that return an
`IAsyncEnumerable<>` auto-follow the continuation token.

## Authentication (root client)

| Client method | HTTP verb | Path / URL | Notes |
| --- | --- | --- | --- |
| `AuthenticateAsync` | POST | `/oauth2/token` | grant_type `client_credentials`; result is cached + auto-refreshed |
| `RefreshTokenAsync` | POST | `/oauth2/token` | grant_type `refresh_token` |

## Organizations - `client.Organizations`

| Client method | HTTP verb | Path / URL | Notes |
| --- | --- | --- | --- |
| `GetAsync` | GET | `/organizations/{orgSlug}` | public organization details |

## Forms - `client.Forms`

| Client method | HTTP verb | Path / URL | Notes |
| --- | --- | --- | --- |
| `ListAsync` | GET | `/organizations/{orgSlug}/forms` | filters: `states`, `formTypes`, `pageIndex`, `pageSize`, `continuationToken` |
| `ListAllAsync` | GET | `/organizations/{orgSlug}/forms` | auto-pager over the above |
| `GetPublicDetailsAsync` | GET | `/organizations/{orgSlug}/forms/{formType}/{formSlug}/public` | public form data |
| `GetTypesAsync` | GET | `/organizations/{orgSlug}/formtypes` | optional `states` filter |
| `GetItemsAsync` | GET | `/organizations/{orgSlug}/forms/{formType}/{formSlug}/items` | item listing + filters |
| `GetOrdersAsync` | GET | `/organizations/{orgSlug}/forms/{formType}/{formSlug}/orders` | order listing + filters |
| `GetPaymentsAsync` | GET | `/organizations/{orgSlug}/forms/{formType}/{formSlug}/payments` | payment listing + filters |
| `GetStatsAsync` | GET | `/organizations/{orgSlug}/forms/{formType}/{formSlug}/stats` | aggregated form statistics |

## Orders - `client.Orders`

| Client method | HTTP verb | Path / URL | Notes |
| --- | --- | --- | --- |
| `GetAsync` | GET | `/orders/{orderId}` | order and its items |
| `ListForOrganizationAsync` | GET | `/organizations/{orgSlug}/orders` | filters + pagination |
| `ListAllForOrganizationAsync` | GET | `/organizations/{orgSlug}/orders` | auto-pager over the above |
| `GetEventTicketsPdfAsync` | GET | `/orders/{orderId}` then each `item.TicketUrl` | fetches the order, then downloads each ticket PDF |

## Items - `client.Items`

| Client method | HTTP verb | Path / URL | Notes |
| --- | --- | --- | --- |
| `GetAsync` | GET | `/items/{itemId}` | single item detail |
| `ListForOrganizationAsync` | GET | `/organizations/{orgSlug}/items` | item listing + filters |
| `ListAllForOrganizationAsync` | GET | `/organizations/{orgSlug}/items` | auto-pager over the above |

## Payments - `client.Payments`

| Client method | HTTP verb | Path / URL | Notes |
| --- | --- | --- | --- |
| `SearchAsync` | GET | `/organizations/{orgSlug}/payments` | filters: `userSearchKey`, `states`, `from`, `to`, pagination |
| `SearchAllAsync` | GET | `/organizations/{orgSlug}/payments` | auto-pager over the above |
| `SearchForUserAsync` | GET | `/organizations/{orgSlug}/payments` | convenience: `userSearchKey={email}&states=Authorized` |
| `GetAsync` | GET | `/payments/{paymentId}` | optional `?withFailedRefundOperation=true` |
| `GetReceiptPdfAsync` | GET | `{payment.PaymentReceiptUrl}` | dynamic URL; requires an elevated token (see note below) |

## Checkout - `client.Checkout`

| Client method | HTTP verb | Path / URL | Notes |
| --- | --- | --- | --- |
| `GetIntentAsync` | GET | `/organizations/{orgSlug}/checkout-intents/{id}` | checkout intent + resulting order once authorized |

## Directory - `client.Directory`

| Client method | HTTP verb | Path / URL | Notes |
| --- | --- | --- | --- |
| `SearchFormsAsync` | POST | `/directory/forms` | filters in body; `pageSize` + `continuationToken` in query |
| `SearchOrganizationsAsync` | POST | `/directory/organizations` | filters in body; continuation-token only (totals are -1) |

## Partners / Users - `client.Partners`, `client.Users`

| Client method | HTTP verb | Path / URL | Notes |
| --- | --- | --- | --- |
| `Partners.GetMeAsync` | GET | `/partners/me` | partner (API client) information |
| `Partners.GetOrganizationsAsync` | GET | `/partners/me/organizations` | partner-linked organizations |
| `Users.GetMyOrganizationsAsync` | GET | `/users/me/organizations` | organizations the current user has rights on |

## Values (reference data) - `client.Values`

| Client method | HTTP verb | Path / URL | Notes |
| --- | --- | --- | --- |
| `GetCompanyLegalStatusesAsync` | GET | `/values/company-legal-status` | |
| `GetOrganizationCategoriesAsync` | GET | `/values/organization-categories` | |
| `GetTagsAsync` | GET | `/values/tags` | |
| `GetFormSubTypesAsync` | GET | `/values/forms/{formType}/types` | |

## CashOut - `client.CashOut`

| Client method | HTTP verb | Path / URL | Notes |
| --- | --- | --- | --- |
| `GetExportAsync` | GET | `/organizations/{orgSlug}/cash-out/{cashOutId}/export` | raw export stream |

## Notifications (webhooks) - `client.Notifications`

| Client method | HTTP verb | Path / URL | Notes |
| --- | --- | --- | --- |
| `Parse` | - | - | parses a raw webhook body into a typed notification |
| `VerifyAuthenticityAsync` | GET | `/payments/{id}` or `/orders/{id}` | re-fetches the referenced resource to confirm authenticity |

## Notes

### Organization-scoped vs global endpoints
- Organization-scoped endpoints (`/organizations/{orgSlug}/...`) operate on a
  single association identified by its slug.
- Global endpoints (`/payments/{paymentId}`, `/orders/{orderId}`,
  `/items/{itemId}`) address a resource directly by id and do not carry the
  organization slug.

### PDF (receipt / ticket) permissions caveat
`Payments.GetReceiptPdfAsync` and `Orders.GetEventTicketsPdfAsync` hit dynamic
URLs that require broader permissions than a JWT obtained from an API key alone
provides. As noted in the [README](../Readme.md), the client still exposes these
calls so they can be enabled more easily if HelloAsso grants fine-grained rights
in the future.

### Directory / Values privileges
Directory and Values endpoints require specific privileges on the API client
(`FormOpenDirectory` / `OrganizationOpenDirectory` / `AccessPublicData`). The
exact directory request body shape is a best-effort mapping and may evolve.
