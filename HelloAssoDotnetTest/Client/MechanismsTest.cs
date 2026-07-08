using System.Net;
using System.Text;
using HelloAssoDotnet.Client;
using HelloAssoDotnet.Models.HelloAssoApi.Auth;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace HelloAssoDotnetTest.Client;

/// <summary>
/// Tests covering the Phase 0 cross-cutting mechanisms: token caching + auto-refresh, the pagination
/// auto-pager and the production/sandbox environment switch.
/// </summary>
public class MechanismsTest
{
    private static IConfiguration GetConfiguration(string environment = "Production")
    {
        var appSettings = $@"{{
                ""HelloAsso"":{{
                    ""SecretsFile"" : ""test secret file"",
                    ""OrganizationSlug"" : ""test-org-slug"",
                    ""Environment"" : ""{environment}""
                  }}
            }}";
        var builder = new ConfigurationBuilder();
        builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));
        return builder.Build();
    }

    private static Mock<IHelloAssoSecretsService> BuildSecretsService()
    {
        var service = new Mock<IHelloAssoSecretsService>();
        service.Setup(s => s.GetClientId()).Returns("test id");
        service.Setup(s => s.GetClientSecret()).Returns("test secret");
        return service;
    }

    private static string TokenJson(int expiresIn)
    {
        return $@"{{
            ""access_token"" : ""cached token"",
            ""expires_in"" : {expiresIn},
            ""refresh_token"" : ""refresh token"",
            ""token_type"" : ""bearer""
        }}";
    }

    private static bool IsOauth(HttpRequestMessage request) => request.RequestUri!.ToString().Contains("oauth2/token");

    private static HelloAssoClient BuildClient(HttpMessageHandler handler, IConfiguration configuration)
    {
        var logger = new Mock<ILogger<HelloAssoClient>>();
        var secrets = BuildSecretsService();
        var httpClient = new HttpClient(handler);
        return new HelloAssoClient(httpClient, secrets.Object, logger.Object, configuration);
    }

    /// <summary>
    /// With a long-lived cached token, calling several endpoints must only authenticate once.
    /// </summary>
    [Test]
    public async Task TokenCache_AuthenticatesOnceAcrossCalls()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => IsOauth(r)), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(TokenJson(1800)) });

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => !IsOauth(r)), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(@"{ ""name"" : ""Test Org"" }") });

        var client = BuildClient(handler.Object, GetConfiguration());

        for (int i = 0; i < 3; i++)
        {
            var result = await client.Organizations.GetAsync();
            Assert.That(result.IsOk, Is.True);
        }

        handler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(r => IsOauth(r)), ItExpr.IsAny<CancellationToken>());
    }

    /// <summary>
    /// A near-instant expiry forces the client to renew the token before the next call.
    /// </summary>
    [Test]
    public async Task TokenCache_RefreshesWhenExpired()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => IsOauth(r)), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(TokenJson(0)) });

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => !IsOauth(r)), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(@"{ ""name"" : ""Test Org"" }") });

        var client = BuildClient(handler.Object, GetConfiguration());

        await client.Organizations.GetAsync();
        await client.Organizations.GetAsync();

        // First call authenticates (client_credentials), second renews the expired token (refresh_token).
        handler.Protected().Verify("SendAsync", Times.Exactly(2), ItExpr.Is<HttpRequestMessage>(r => IsOauth(r)), ItExpr.IsAny<CancellationToken>());
    }

    /// <summary>
    /// Supplying explicit tokens bypasses the cache entirely: no OAuth round-trip happens.
    /// </summary>
    [Test]
    public async Task ExplicitTokens_BypassCache()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(@"{ ""name"" : ""Test Org"" }") });

        var client = BuildClient(handler.Object, GetConfiguration());

        var explicitTokens = new AuthTokens { AccessToken = "explicit token" };
        var result = await client.Organizations.GetAsync(explicitTokens);

        Assert.That(result.IsOk, Is.True);
        handler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>(r => IsOauth(r)), ItExpr.IsAny<CancellationToken>());
    }

    /// <summary>
    /// The auto-pager stitches multiple pages and stops on the first empty page.
    /// </summary>
    [Test]
    public async Task Pager_StitchesPagesAndStopsOnEmpty()
    {
        var page1 = @"{ ""data"" : [ { ""id"" : 1 }, { ""id"" : 2 } ], ""pagination"" : { ""continuationToken"" : ""c1"", ""pageIndex"" : 1, ""pageSize"" : 2, ""totalCount"" : 3, ""totalPages"" : 2 } }";
        var page2 = @"{ ""data"" : [ { ""id"" : 3 } ], ""pagination"" : { ""continuationToken"" : ""c2"", ""pageIndex"" : 2, ""pageSize"" : 2, ""totalCount"" : 3, ""totalPages"" : 2 } }";
        var page3 = @"{ ""data"" : [], ""pagination"" : { ""continuationToken"" : """" } }";

        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(page1) })
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(page2) })
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(page3) });

        var client = BuildClient(handler.Object, GetConfiguration());
        var explicitTokens = new AuthTokens { AccessToken = "explicit token" };

        var ids = new List<int>();
        await foreach (var order in client.Orders.ListAllForOrganizationAsync(new ListOrdersRequest(), explicitTokens))
        {
            ids.Add(order.Id);
        }

        Assert.That(ids, Is.EquivalentTo(new[] { 1, 2, 3 }));
    }

    /// <summary>
    /// Configuring the Sandbox environment must target the sandbox host.
    /// </summary>
    [Test]
    public async Task Sandbox_TargetsSandboxHost()
    {
        Uri? capturedUri = null;
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedUri = request.RequestUri)
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(@"{ ""name"" : ""Test Org"" }") });

        var client = BuildClient(handler.Object, GetConfiguration("Sandbox"));
        var explicitTokens = new AuthTokens { AccessToken = "explicit token" };

        await client.Organizations.GetAsync(explicitTokens);

        Assert.That(capturedUri, Is.Not.Null);
        Assert.That(capturedUri!.Host, Is.EqualTo("api.helloasso-sandbox.com"));
    }
}
