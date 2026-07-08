using System.Net;
using System.Text;
using System.Text.Json;
using HelloAssoDotnet.Client;
using HelloAssoDotnet.Models.Api.Auth;
using HelloAssoDotnet.Models.Api.Notifications;
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

    private static IHelloAssoClient BuildClient(HttpMessageHandler handler)
    {
        var secrets = new Mock<IHelloAssoSecretsService>();
        secrets.Setup(s => s.GetClientId()).Returns("test id");
        secrets.Setup(s => s.GetClientSecret()).Returns("test secret");
        return TestClientFactory.Build(handler, secrets.Object, GetConfiguration());
    }

    private static IHelloAssoClient BuildClientReturning(HttpStatusCode statusCode)
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
    public void ReadData_PaymentNotification_TypedPayload()
    {
        var client = BuildClientReturning(HttpStatusCode.OK);
        var body = @"{ ""eventType"" : ""Payment"", ""data"" : { ""id"" : 42, ""amount"" : 8000, ""order"" : { ""id"" : 7 }, ""payer"" : { } } }";
        var parsed = client.Notifications.Parse(body);

        var payload = client.Notifications.ReadData(parsed.Value!);

        Assert.That(payload, Is.InstanceOf<PaymentNotificationPayload>());
        var payment = ((PaymentNotificationPayload)payload!).Payment;
        Assert.That(payment.Id, Is.EqualTo(42));
        Assert.That(payment.Amount, Is.EqualTo(8000));
        Assert.That(payment.Order.Id, Is.EqualTo(7));
    }

    [Test]
    public void ReadData_OrderNotification_TypedPayload()
    {
        var client = BuildClientReturning(HttpStatusCode.OK);
        var body = @"{ ""eventType"" : ""Order"", ""data"" : { ""id"" : 7, ""formType"" : ""Event"", ""formSlug"" : ""my-form"" } }";
        var parsed = client.Notifications.Parse(body);

        var payload = client.Notifications.ReadData(parsed.Value!);

        Assert.That(payload, Is.InstanceOf<OrderNotificationPayload>());
        var order = ((OrderNotificationPayload)payload!).Order;
        Assert.That(order.Id, Is.EqualTo(7));
        Assert.That(order.FormSlug, Is.EqualTo("my-form"));
    }

    [Test]
    public void ReadData_UnknownEventType_ReturnsNull_RawDataPreserved()
    {
        var client = BuildClientReturning(HttpStatusCode.OK);
        var body = @"{ ""eventType"" : ""SomethingBrandNew"", ""data"" : { ""id"" : 99 } }";
        var parsed = client.Notifications.Parse(body);

        var payload = client.Notifications.ReadData(parsed.Value!);

        Assert.That(payload, Is.Null);
        // The raw payload must remain readable as an escape hatch.
        Assert.That(parsed.Value!.EventType, Is.EqualTo(NotificationEventType.Unknown));
        Assert.That(parsed.Value.Data.GetProperty("id").GetInt32(), Is.EqualTo(99));
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
