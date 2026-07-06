# Mapped HelloAsso endpoints

This is the set of HelloAsso REST API endpoints currently mapped by
[`HelloAssoClient`](../HelloAssoDotnet/Client/HelloAssoClient.cs). It is not an
exhaustive mapping of the HelloAsso API - only what the library exposes today.

- API base URL: `https://api.helloasso.com/v5`
- OAuth base URL: `https://api.helloasso.com/oauth2`
- `{orgSlug}` is taken from configuration (`HelloAsso:OrganizationSlug`).

| Client method | HTTP verb | Path / URL | Notes |
| --- | --- | --- | --- |
| `AuthenticateAsync` | POST | `/oauth2/token` | grant_type `client_credentials` (from ClientId / ClientSecret) |
| `RefreshTokenAsync` | POST | `/oauth2/token` | grant_type `refresh_token` |
| `GetPaymentForUserAsync` | GET | `/v5/organizations/{orgSlug}/payments` | query `userSearchKey={email}&states=Authorized` |
| `GetFormsFromOrganization` | GET | `/v5/organizations/{orgSlug}/forms` | optional query `formTypes`, `pageIndex`, `pageSize`, `continuationToken` |
| `GetFormDetailsAsync` | GET | `/v5/organizations/{orgSlug}/forms/{formType}/{formSlug}/public` | public form data |
| `GetPaymentDetailsAsync` | GET | `/v5/payments/{paymentId}` | optional query `?withFailedRefundOperation=true` |
| `GetOrderDetailsAsync` | GET | `/v5/orders/{orderId}` | order and its items |
| `GetPaymentReceiptPdfAsync` | GET | `{payment.PaymentReceiptUrl}` | dynamic URL from the payment; requires an elevated token (see note below) |
| `GetEventTicketPdf` | GET | `/v5/orders/{orderId}` then each `item.TicketUrl` | fetches the order, then downloads each ticket PDF from its dynamic URL |

## Notes

### Organization-scoped vs global endpoints
- Organization-scoped endpoints (`/organizations/{orgSlug}/...`) operate on a
  single association identified by its slug: payments and forms.
- Global endpoints (`/payments/{paymentId}`, `/orders/{orderId}`) address a
  resource directly by id and do not carry the organization slug.

### PDF (receipt / ticket) permissions caveat
`GetPaymentReceiptPdfAsync` and `GetEventTicketPdf` hit dynamic URLs that
require broader permissions than a JWT obtained from an API key alone provides.
As noted in the [README](../Readme.md), the client still exposes these calls so
they can be enabled more easily if HelloAsso grants fine-grained rights in the
future.
