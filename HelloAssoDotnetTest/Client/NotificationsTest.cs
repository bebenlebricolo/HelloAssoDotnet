using System.Net;
using System.Text;
using System.Text.Json;
using HelloAssoDotnet.Client;
using HelloAssoDotnet.Models.HelloAssoApi.Auth;
using HelloAssoDotnet.Models.HelloAssoApi.Notifications;
using HelloAssoDotnet.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace HelloAssoDotnetTest.Client;

/// <summary>
/// Tests covering the Phase 2 notifications (webhooks) helpers: parsing and authenticity verification.
/// </summary>
public class NotificationsTest
{
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

    private static HelloAssoClient BuildClient(HttpMessageHandler handler)
    {
        var logger = new Mock<ILogger<HelloAssoClient>>();
        var secrets = new Mock<IHelloAssoSecretsService>();
        secrets.Setup(s => s.GetClientId()).Returns("test id");
        secrets.Setup(s => s.GetClientSecret()).Returns("test secret");
        var httpClient = new HttpClient(handler);
        return new HelloAssoClient(httpClient, secrets.Object, logger.Object, GetConfiguration());
    }

    private static HelloAssoClient BuildClientReturning(HttpStatusCode statusCode)
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(statusCode) { Content = new StringContent(@"{ ""id"" : 42 }") });
        return BuildClient(handler.Object);
    }

    [Test]
    public void Parse_PaymentNotification_Ok()
    {
        var client = BuildClientReturning(HttpStatusCode.OK);
        var body = @"{ ""eventType"" : ""Payment"", ""data"" : { ""id"" : 42, ""amount"" : 8000 }, ""metadata"" : { ""foo"" : ""bar"" } }";

        var result = client.Notifications.Parse(body);

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.Value!.EventType, Is.EqualTo(NotificationEventType.Payment));
        Assert.That(result.Value.Data.GetProperty("id").GetInt32(), Is.EqualTo(42));
        Assert.That(result.Value.Data.GetProperty("amount").GetInt32(), Is.EqualTo(8000));
        Assert.That(result.Value.Metadata, Is.Not.Null);
        Assert.That(result.Value.Metadata!.Value.GetProperty("foo").GetString(), Is.EqualTo("bar"));
    }

    [Test]
    public void Parse_OrderNotification_Ok()
    {
        var client = BuildClientReturning(HttpStatusCode.OK);
        var body = @"{ ""eventType"" : ""Order"", ""data"" : { ""id"" : 7 } }";

        var result = client.Notifications.Parse(body);

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.Value!.EventType, Is.EqualTo(NotificationEventType.Order));
        Assert.That(result.Value.Metadata, Is.Null);
    }

    [Test]
    public void Parse_InvalidBody_ClientError()
    {
        var client = BuildClientReturning(HttpStatusCode.OK);

        var result = client.Notifications.Parse("this is not json");

        Assert.That(result.IsOk, Is.False);
    }

    [Test]
    public async Task Verify_PaymentReFetchSucceeds_IsAuthentic()
    {
        var client = BuildClientReturning(HttpStatusCode.OK);
        var parsed = client.Notifications.Parse(@"{ ""eventType"" : ""Payment"", ""data"" : { ""id"" : 42 } }");

        var verification = await client.Notifications.VerifyAuthenticityAsync(parsed.Value!, new AuthTokens { AccessToken = "explicit" });

        Assert.That(verification.IsOk, Is.True);
        Assert.That(verification.Value!.IsAuthentic, Is.True);
    }

    [Test]
    public async Task Verify_PaymentReFetchFails_IsNotAuthentic()
    {
        var client = BuildClientReturning(HttpStatusCode.NotFound);
        var parsed = client.Notifications.Parse(@"{ ""eventType"" : ""Payment"", ""data"" : { ""id"" : 42 } }");

        var verification = await client.Notifications.VerifyAuthenticityAsync(parsed.Value!, new AuthTokens { AccessToken = "explicit" });

        Assert.That(verification.IsOk, Is.True);
        Assert.That(verification.Value!.IsAuthentic, Is.False);
        Assert.That(verification.Value.Reason, Is.Not.Null);
    }
}
