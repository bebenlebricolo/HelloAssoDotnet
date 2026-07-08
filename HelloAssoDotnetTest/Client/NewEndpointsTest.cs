using System.Net;
using System.Text;
using HelloAssoDotnet.Client;
using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.Api.Forms;
using HelloAssoDotnet.Models.Api.Payment;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace HelloAssoDotnetTest.Client;

/// <summary>
/// Focused tests for a representative subset of the new read-only endpoints (deserialization, query
/// building and POST body handling). Explicit tokens are used to isolate the endpoint under test from the
/// authentication round-trip.
/// </summary>
public class NewEndpointsTest
{
    private static readonly AuthTokens Tokens = new AuthTokens { AccessToken = "explicit token" };

    private static IConfiguration GetConfiguration()
    {
        var appSettings = @"{
                ""HelloAsso"":{
                    ""SecretsFile"" : ""test secret file"",
                    ""OrganizationSlug"" : ""test-org-slug""
                  }
            }";
        var builder = new ConfigurationBuilder();
        builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));
        return builder.Build();
    }

    /// <summary>
    /// Builds a client whose handler always returns the given JSON, and captures the last request seen.
    /// </summary>
    private static HelloAssoClient BuildClient(string json, out Func<HttpRequestMessage?> lastRequest, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        HttpRequestMessage? captured = null;
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => captured = request)
            .ReturnsAsync(() => new HttpResponseMessage(statusCode) { Content = new StringContent(json) });

        var logger = new Mock<ILogger<HelloAssoClient>>();
        var secrets = new Mock<IHelloAssoSecretsService>();
        secrets.Setup(s => s.GetClientId()).Returns("test id");
        secrets.Setup(s => s.GetClientSecret()).Returns("test secret");
        var httpClient = new HttpClient(handler.Object);

        lastRequest = () => captured;
        return new HelloAssoClient(httpClient, secrets.Object, logger.Object, GetConfiguration());
    }

    [Test]
    public async Task Organizations_Get_Ok()
    {
        var client = BuildClient(@"{ ""name"" : ""Test Org"", ""organizationSlug"" : ""test-org-slug"", ""type"" : ""Association1901"" }", out var lastRequest);

        var result = await client.Organizations.GetAsync(Tokens);

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.Value!.Name, Is.EqualTo("Test Org"));
        Assert.That(result.Value.OrganizationSlug, Is.EqualTo("test-org-slug"));
        Assert.That(lastRequest()!.RequestUri!.ToString(), Does.EndWith("/organizations/test-org-slug"));
    }

    [Test]
    public async Task Orders_ListForOrganization_BuildsQuery()
    {
        var client = BuildClient(@"{ ""data"" : [ { ""id"" : 1 } ], ""pagination"" : { ""continuationToken"" : """" } }", out var lastRequest);

        var request = new ListOrdersRequest
        {
            FormTypes = new List<FormType> { FormType.Event },
            PageSize = 50,
        };
        var result = await client.Orders.ListForOrganizationAsync(request, Tokens);

        Assert.That(result.IsOk, Is.True);
        var url = lastRequest()!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("/organizations/test-org-slug/orders"));
        Assert.That(url, Does.Contain("formTypes=Event"));
        Assert.That(url, Does.Contain("pageSize=50"));
    }

    [Test]
    public async Task Payments_Search_BuildsFilters()
    {
        var client = BuildClient(@"{ ""data"" : [], ""pagination"" : { ""continuationToken"" : """" } }", out var lastRequest);

        var request = new SearchPaymentsRequest
        {
            UserSearchKey = "someone@somewhere.com",
            States = new List<PaymentState> { PaymentState.Authorized, PaymentState.Refunded },
        };
        var result = await client.Payments.SearchAsync(request, Tokens);

        Assert.That(result.IsOk, Is.True);
        var url = lastRequest()!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("userSearchKey="));
        Assert.That(url, Does.Contain("states=Authorized"));
        Assert.That(url, Does.Contain("states=Refunded"));
    }

    [Test]
    public async Task Items_Get_Ok()
    {
        var client = BuildClient(@"{ ""id"" : 999, ""ticketUrl"" : ""https://example.com/t"" }", out var lastRequest);

        var result = await client.Items.GetAsync(999, Tokens);

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.Value!.Id, Is.EqualTo(999));
        Assert.That(lastRequest()!.RequestUri!.ToString(), Does.EndWith("/items/999"));
    }

    [Test]
    public async Task Checkout_GetIntent_Ok()
    {
        var client = BuildClient(@"{ ""id"" : 12345, ""redirectUrl"" : ""https://example.com/pay"" }", out var lastRequest);

        var result = await client.Checkout.GetIntentAsync(12345, Tokens);

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.Value!.Id, Is.EqualTo(12345));
        Assert.That(lastRequest()!.RequestUri!.ToString(), Does.EndWith("/organizations/test-org-slug/checkout-intents/12345"));
    }

    [Test]
    public async Task Values_GetTags_Ok()
    {
        var client = BuildClient(@"[ { ""name"" : ""sport"" }, { ""name"" : ""culture"" } ]", out _);

        var result = await client.Values.GetTagsAsync(Tokens);

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.Value!.Count, Is.EqualTo(2));
        Assert.That(result.Value[0].Name, Is.EqualTo("sport"));
    }

    [Test]
    public async Task Directory_SearchForms_PostsBody()
    {
        var client = BuildClient(@"{ ""data"" : [], ""pagination"" : { ""continuationToken"" : ""tok"" } }", out var lastRequest);

        var request = new DirectoryFormsRequest
        {
            PageSize = 20,
            Filters = new DirectoryFormsFilters { FormTypes = new List<FormType> { FormType.Event } },
        };
        var result = await client.Directory.SearchFormsAsync(request, Tokens);

        Assert.That(result.IsOk, Is.True);
        var httpRequest = lastRequest()!;
        Assert.That(httpRequest.Method, Is.EqualTo(HttpMethod.Post));
        Assert.That(httpRequest.RequestUri!.ToString(), Does.Contain("/directory/forms"));
        Assert.That(httpRequest.RequestUri!.ToString(), Does.Contain("pageSize=20"));
        Assert.That(httpRequest.Content, Is.Not.Null);
    }

    [Test]
    public async Task Partners_GetMe_Ok()
    {
        var client = BuildClient(@"{ ""name"" : ""My Partner"", ""apiClient"" : { ""privileges"" : [ ""AccessPublicData"" ], ""domain"" : ""example.com"" } }", out var lastRequest);

        var result = await client.Partners.GetMeAsync(Tokens);

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.Value!.Name, Is.EqualTo("My Partner"));
        Assert.That(result.Value.ApiClient!.Privileges, Does.Contain("AccessPublicData"));
        Assert.That(result.Value.ApiClient.Domain, Is.EqualTo("example.com"));
        Assert.That(lastRequest()!.RequestUri!.ToString(), Does.EndWith("/partners/me"));
    }

    [Test]
    public async Task Forms_GetStats_Ok()
    {
        var client = BuildClient(@"{ ""totalParticipant"" : 42, ""unGroupedTiers"" : [ { ""id"" : 1, ""label"" : ""Standard"", ""entriesTaken"" : 10, ""price"" : 1500 } ], ""additionalOptions"" : [] }", out var lastRequest);

        var result = await client.Forms.GetStatsAsync(FormType.Event, "my-form", Tokens);

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.Value!.TotalParticipant, Is.EqualTo(42));
        Assert.That(result.Value.UnGroupedTiers, Has.Count.EqualTo(1));
        Assert.That(result.Value.UnGroupedTiers[0].Label, Is.EqualTo("Standard"));
        Assert.That(result.Value.UnGroupedTiers[0].Price, Is.EqualTo(1500));
        Assert.That(lastRequest()!.RequestUri!.ToString(), Does.EndWith("/organizations/test-org-slug/forms/Event/my-form/stats"));
    }

    [Test]
    public async Task CashOut_GetExport_Ok()
    {
        var client = BuildClient(@"raw export content", out var lastRequest);

        var result = await client.CashOut.GetExportAsync("cashout-1", Tokens);

        Assert.That(result.IsOk, Is.True);
        Assert.That(lastRequest()!.RequestUri!.ToString(), Does.EndWith("/organizations/test-org-slug/cash-out/cashout-1/export"));
    }
}
