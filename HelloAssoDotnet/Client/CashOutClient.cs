using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Utils;
using Microsoft.Extensions.Logging;

namespace HelloAssoDotnet.Client;

/// <inheritdoc cref="ICashOutClient" />
internal sealed class CashOutClient : HelloAssoSubClient, ICashOutClient
{
    public CashOutClient(IHelloAssoClientContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Result<Stream>> GetExportAsync(string cashOutId, AuthTokens? tokens = null, CancellationToken cancellationToken = default)
    {
        var accessToken = await ResolveAccessTokenAsync(tokens, cancellationToken);
        if (!accessToken.IsOk)
        {
            return Result<Stream>.FromError(accessToken.Error);
        }

        var url = $"{Context.BaseUri}/organizations/{Context.OrganizationSlug}/cash-out/{cashOutId}/export";
        var request = new HttpRequestMessage(HttpMethod.Get, url)
            .WithBearer(accessToken.Value!)
            .WithUserAgent(Context.Config);

        var response = await Context.HttpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            Context.Logger.LogError($"Failed to get cash-out export for {cashOutId}");
            return Result<Stream>.FromHttpResponse(response);
        }

        return Result<Stream>.Ok(await response.Content.ReadAsStreamAsync(cancellationToken));
    }
}
