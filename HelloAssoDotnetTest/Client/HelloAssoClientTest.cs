using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using HelloAssoDotnet.Client;
using HelloAssoDotnet.Models.Configuration;
using HelloAssoDotnet.Models.HelloAssoApi.Auth;
using HelloAssoDotnet.Models.HelloAssoApi.Forms;
using HelloAssoDotnet.Models.HelloAssoApi.Order;
using HelloAssoDotnet.Models.HelloAssoApi.Payment;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Services;
using HelloAssoDotnet.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace HelloAssoDotnetTest.Client;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    private static IConfiguration GetConfiguration()
    {
        var appSettings = @"{
                ""HelloAsso"":{
                    ""SecretsFile"" : ""test secret file"",
                    ""OrganizationSlug"" : ""test org slug"",
                  }
            }";
        var builder = new ConfigurationBuilder();
        builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

        var configuration = builder.Build();
        return configuration;
    }

    private void SetupMessageHandlerForDefault(ref Mock<HttpMessageHandler> handler)
    {
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.IsAny<HttpRequestMessage>(),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Default"),
            });
    }

    private void SetupSecretsService(ref Mock<IHelloAssoSecretsService> service)
    {
        service.Setup(s => s.GetClientId()).Returns("test id");
        service.Setup(s => s.GetClientSecret()).Returns("test secret");
    }

    private async Task<string> ReadEmbeddedResource(string filename)
    {
        string resourceName = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith(filename));
        string resourceJson = "";
        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)!)
        using (StreamReader reader = new StreamReader(stream))
        {
            resourceJson = await reader.ReadToEndAsync();
        }

        return resourceJson;
    }

    private void SetupMessageHandlerBasicError(ref Mock<HttpMessageHandler> handler, HttpStatusCode statusCode)
    {
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.IsAny<HttpRequestMessage>(),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(statusCode));
    }

    /// <summary>
    /// Auth request should properly deserialize http response payload data model
    /// </summary>
    [Test]
    public async Task ClientAuth_Ok()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        SetupMessageHandlerForDefault(ref mockHttpMessageHandler);

        string expectedToken = "some token";
        string expectedRefreshToken = "some refresh token";
        int expectedExpiresIn = 1800;
        string expectedTokenType = "bearer";
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.IsAny<HttpRequestMessage>(),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($@"{{
                                                ""access_token"" : ""{expectedToken}"",
                                                ""expires_in"" : {expectedExpiresIn},
                                                ""refresh_token"" : ""{expectedRefreshToken}"",
                                                ""token_type"" : ""{expectedTokenType}""
                                            }}")
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());

        var result = await client.AuthenticateAsync();
        Assert.That(result.IsOk, Is.True);

        var authTokens = result.Value;
        Assert.That(authTokens, Is.Not.Null);
        Assert.That(authTokens.AccessToken, Is.EqualTo(expectedToken));
        Assert.That(authTokens.RefreshToken, Is.EqualTo(expectedRefreshToken));
        Assert.That(authTokens.ExpiresIn, Is.EqualTo(expectedExpiresIn));
        Assert.That(authTokens.TokenType, Is.EqualTo(expectedTokenType));
    }

    /// <summary>
    /// This Auth request should return null objects in case of unauthorized.
    /// </summary>
    [Test]
    public async Task ClientAuth_Unauthorized()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        SetupMessageHandlerBasicError(ref mockHttpMessageHandler, HttpStatusCode.Unauthorized);

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());
        var result = await client.AuthenticateAsync();
        Assert.That(result.IsOk, Is.False);
        Assert.That(result.Error, Is.EqualTo(Errors.Unauthenticated));
    }

    /// <summary>
    /// This Refresh token request should return null objects in case of unauthorized.
    /// </summary>
    [Test]
    public async Task ClientTokenRefresh_Unauthorized()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        SetupMessageHandlerBasicError(ref mockHttpMessageHandler, HttpStatusCode.Unauthorized);

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());
        var result = await client.AuthenticateAsync();
        Assert.That(result.IsOk, Is.False);
        Assert.That(result.Error, Is.EqualTo(Errors.Unauthenticated));
    }

    /// <summary>
    /// Happy path scenario where we can retrieve user's payments
    /// </summary>
    [Test]
    public async Task GetPaymentForUserAsync_OK()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        SetupMessageHandlerForDefault(ref mockHttpMessageHandler);

        string resourceJson = await ReadEmbeddedResource("PaymentPayloadOkTest.json");
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.IsAny<HttpRequestMessage>(),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(resourceJson)
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());
        var result = await client.GetPaymentForUserAsync("test email", new AuthTokens());
        var payments = result.Value;
        Assert.That(result.IsOk, Is.True);

        Assert.That(payments!.Data.Count, Is.EqualTo(1));
        Assert.That(payments.Data[0].Amount, Is.EqualTo(8000));
        Assert.That(payments.Data[0].Payer.Email, Is.EqualTo("someone@somewhere.com"));
        Assert.That(payments.Data[0].Payer.Country, Is.EqualTo("FRA"));
        Assert.That(payments.Data[0].Payer.FirstName, Is.EqualTo("test name"));
        Assert.That(payments.Data[0].Payer.LastName, Is.EqualTo("test lastname"));
        Assert.That(payments.Data[0].Items!.Count, Is.EqualTo(1));
    }

    /// <summary>
    /// What happens when user data can't be found ?
    /// </summary>
    [Test]
    public async Task GetPaymentForUserAsync_NotFound()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        SetupMessageHandlerBasicError(ref mockHttpMessageHandler, HttpStatusCode.NotFound);

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());
        var result = await client.GetPaymentForUserAsync("test email", new AuthTokens());
        Assert.That(result.IsOk, Is.False);
        Assert.That(result.Error, Is.EqualTo(Errors.NotFound));

        var payments = result.Value;
        Assert.That(payments, Is.Null);
    }

    #region PdfReceiptTests

    private AuthTokens GenerateFakeTokens()
    {
        return new AuthTokens()
        {
            AccessToken = "fake token",
            RefreshToken = "fake refresh token",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };
    }

    private PaymentResponse GenerateFakePaymentResponse()
    {
        return new PaymentResponse
        {
            Id = 123,
            Order = new Order(),
            Payer = new Payer(),
            PaymentReceiptUrl = "https://www.example.com",
        };
    }

    /// <summary>
    /// Retrieving raw Stream in case of success
    /// </summary>
    [Test]
    public async Task GetReceiptPdfAsync_OK()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        var fakeStream = new MemoryStream();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.IsAny<HttpRequestMessage>(),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(fakeStream)
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());
        var fakePayment = GenerateFakePaymentResponse();
        var fakeTokens = GenerateFakeTokens();

        var result = await client.GetPaymentReceiptPdfAsync(fakePayment, fakeTokens);
        Assert.That(result.IsOk, Is.True);
        Assert.That(result.Value, Is.EqualTo(fakeStream));
    }

    /// <summary>
    /// Abstracts Http Unauthorized response
    /// </summary>
    [Test]
    public async Task GetReceiptPdfAsync_Unauthorized()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        SetupMessageHandlerBasicError(ref mockHttpMessageHandler, HttpStatusCode.Unauthorized);

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());
        var fakePayment = GenerateFakePaymentResponse();
        var fakeTokens = GenerateFakeTokens();

        var result = await client.GetPaymentReceiptPdfAsync(fakePayment, fakeTokens);
        Assert.That(result.IsOk, Is.False);
        Assert.That(result.Error, Is.EqualTo(Errors.Unauthenticated));
    }

    /// <summary>
    /// Abstract NotFound http response
    /// </summary>
    [Test]
    public async Task? GetReceiptPdfAsync_NotFound()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        SetupMessageHandlerBasicError(ref mockHttpMessageHandler, HttpStatusCode.NotFound);

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());
        var fakePayment = GenerateFakePaymentResponse();
        var fakeTokens = GenerateFakeTokens();
        var result = await client.GetPaymentReceiptPdfAsync(fakePayment, fakeTokens);
        Assert.That(result.IsOk, Is.False);
        Assert.That(result.Error, Is.EqualTo(Errors.NotFound));
    }

    #endregion

    #region TicketsPdfTests

    /// <summary>
    /// Retrieves a single Pdf ticket from remote
    /// </summary>
    [Test]
    public async Task GetTicketsPdfAsync_OKSingle()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var fakePayment = GenerateFakePaymentResponse();
        var fakeTokens = GenerateFakeTokens();

        var fakeStream = new MemoryStream();
        var fakeOrderDetails = new OrderDetails()
        {
            Items = new List<OrderItem>()
            {
                new OrderItem()
                {
                    TicketUrl = "https://www.example.com/ticket1",
                }
            }
        };

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("orders/")),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(fakeOrderDetails, JsonOptionsProvider.GetJsonOptions())),
            });

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.Is<HttpRequestMessage>(r => r.RequestUri == new Uri(fakeOrderDetails.Items.First().TicketUrl!)),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(fakeStream)
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());

        var result = await client.GetEventTicketPdf(fakePayment, fakeTokens);
        Assert.That(result.IsOk, Is.True);
        Assert.That(result.Value!.Count, Is.EqualTo(1));
        Assert.That(result.Value[0], Is.EqualTo(fakeStream));
    }

    [Test]
    public async Task GetTicketsPdfAsync_OKMultiple()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var fakePayment = GenerateFakePaymentResponse();
        var fakeTokens = GenerateFakeTokens();

        var fakeStream = new MemoryStream();
        var fakeOrderDetails = new OrderDetails()
        {
            Items = new List<OrderItem>()
            {
                new OrderItem()
                {
                    TicketUrl = "https://www.example.com/ticket1",
                },
                new OrderItem()
                {
                    TicketUrl = "https://www.example.com/ticket2",
                },
            }
        };

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("orders/")),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(fakeOrderDetails, JsonOptionsProvider.GetJsonOptions())),
            });

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("https://www.example.com/ticket")),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(fakeStream)
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());

        var result = await client.GetEventTicketPdf(fakePayment, fakeTokens);
        Assert.That(result.IsOk, Is.True);
        Assert.That(result.Value!.Count, Is.EqualTo(2));

        // We're sending the same FakeStream
        Assert.That(result.Value[0], Is.EqualTo(fakeStream));
        Assert.That(result.Value[1], Is.EqualTo(fakeStream));
    }

    /// <summary>
    /// Retrieves a single Pdf ticket from remote
    /// </summary>
    [Test]
    public async Task GetTicketsPdfAsync_Unauthorized()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var fakePayment = GenerateFakePaymentResponse();
        var fakeTokens = GenerateFakeTokens();
        SetupMessageHandlerBasicError(ref mockHttpMessageHandler, HttpStatusCode.Unauthorized);

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());

        var result = await client.GetEventTicketPdf(fakePayment, fakeTokens);
        Assert.That(result.IsOk, Is.False);
        Assert.That(result.Error, Is.EqualTo(Errors.Unauthenticated));
    }

    /// <summary>
    /// Retrieves a single Pdf ticket from remote
    /// </summary>
    [Test]
    public async Task GetTicketsPdfAsync_NotFound()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var fakePayment = GenerateFakePaymentResponse();
        var fakeTokens = GenerateFakeTokens();
        SetupMessageHandlerBasicError(ref mockHttpMessageHandler, HttpStatusCode.NotFound);

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());

        var result = await client.GetEventTicketPdf(fakePayment, fakeTokens);
        Assert.That(result.IsOk, Is.False);
        Assert.That(result.Error, Is.EqualTo(Errors.NotFound));
    }

    /// <summary>
    /// Retrieves a single Pdf ticket from remote
    /// </summary>
    [Test]
    public async Task GetTicketsPdfAsync_OneTicketIsMissing()
    {
       var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var fakePayment = GenerateFakePaymentResponse();
        var fakeTokens = GenerateFakeTokens();

        var fakeStream = new MemoryStream();
        var fakeOrderDetails = new OrderDetails()
        {
            Items = new List<OrderItem>()
            {
                new OrderItem()
                {
                    TicketUrl = "https://www.example.com/ticket1",
                },
                new OrderItem()
                {
                    TicketUrl = "https://www.example.com/ticket2",
                },
            }
        };

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("orders/")),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(fakeOrderDetails, JsonOptionsProvider.GetJsonOptions())),
            });

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.Is<HttpRequestMessage>(r => r.RequestUri == new Uri(fakeOrderDetails.Items.First().TicketUrl!)),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(fakeStream)
            });

        // Second item will be missing
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.Is<HttpRequestMessage>(r => r.RequestUri == new Uri(fakeOrderDetails.Items[1].TicketUrl!)),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());

        var result = await client.GetEventTicketPdf(fakePayment, fakeTokens);
        Assert.That(result.IsOk, Is.False);
        Assert.That(result.Error, Is.EqualTo(Errors.ServerError));
    }

    #endregion

    #region GetFormDetailsAsync
    [Test]
    public async Task GetFormDetailsAsync_Ok()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        // var fakePayment = GenerateFakePaymentResponse();
        var fakeTokens = GenerateFakeTokens();

        var formDetails = await ReadEmbeddedResource("BlocSessionFormDetails.json");

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("forms/")),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(formDetails),
            });


        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());

        var formSlug = "carte-bloc-session-2";
        var formType = FormType.Event;
        var result = await client.GetFormDetailsAsync(formSlug, formType, fakeTokens);
        Assert.That(result.IsOk, Is.True);

        var retrievedFormDetails = result.Value!;

        // Check tier
        Assert.That(retrievedFormDetails.Tiers!.Count, Is.EqualTo(1));

        var firstTier = retrievedFormDetails.Tiers.First();
        Assert.That(firstTier.Id, Is.EqualTo(13744717));
        Assert.That(firstTier.Label, Is.EqualTo("Test tier label"));
        Assert.That(firstTier.Description, Is.EqualTo("Test tier description"));
        Assert.That(firstTier.TierType, Is.EqualTo(TierType.Registration));
        Assert.That(firstTier.Price, Is.EqualTo(3250));
        Assert.That(firstTier.VatRate, Is.EqualTo(0.00));
        Assert.That(firstTier.PaymentFrequency, Is.EqualTo(PaymentFrequencyEnum.Single));
        Assert.That(firstTier.MaxPerUser, Is.EqualTo(2));
        Assert.That(firstTier.IsEligibleTaxReceipt, Is.False);

        Assert.That(retrievedFormDetails.FormSlug, Is.EqualTo(formSlug));
        Assert.That(retrievedFormDetails.FormType, Is.EqualTo(FormType.Event));
        Assert.That(retrievedFormDetails.OrganizationLogo, Is.EqualTo("https://www.example.com/logo.png"));
        Assert.That(retrievedFormDetails.OrganizationName, Is.EqualTo("Test Org"));
        Assert.That(retrievedFormDetails.ActivityType, Is.EqualTo("Atelier"));
        Assert.That(retrievedFormDetails.ActivityTypeId, Is.EqualTo(2));
        Assert.That(retrievedFormDetails.Place!.Name, Is.EqualTo("Test place"));
        Assert.That(retrievedFormDetails.Place!.Country, Is.EqualTo("FRA"));
        Assert.That(retrievedFormDetails.PersonalizedMessage, Is.EqualTo("Test personalized message"));
        Assert.That(retrievedFormDetails.Currency, Is.EqualTo(CurrencyEnum.Euro));
        Assert.That(retrievedFormDetails.Description, Is.EqualTo("Test form description"));
        Assert.That(retrievedFormDetails.State, Is.EqualTo(PublicationState.Private));
        Assert.That(retrievedFormDetails.Title, Is.EqualTo("Test form title"));
        Assert.That(retrievedFormDetails.WidgetButtonUrl, Is.EqualTo("https://www.somewhere.com/carte-bloc-session-2/widget-bouton"));
        Assert.That(retrievedFormDetails.WidgetFullUrl, Is.EqualTo("https://www.somewhere.com/carte-bloc-session-2/widget"));
        Assert.That(retrievedFormDetails.WidgetVignetteHorizontalUrl, Is.EqualTo("https://www.somewhere.com/carte-bloc-session-2/widget-vignette-horizontale"));
        Assert.That(retrievedFormDetails.WidgetVignetteVerticalUrl, Is.EqualTo("https://www.somewhere.com/carte-bloc-session-2/widget-vignette"));
        Assert.That(retrievedFormDetails.Url, Is.EqualTo("https://www.somewhere.com/carte-bloc-session-2"));
    }

    [Test]
    public async Task GetFormDetailsAsync_Unauthorized()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        // var fakePayment = GenerateFakePaymentResponse();
        var fakeTokens = GenerateFakeTokens();

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("forms/")),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Forbidden));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());

        var result = await client.GetFormDetailsAsync("test-slug", FormType.Event, fakeTokens);
        Assert.That(result.IsOk, Is.False);
        Assert.That(result.Error, Is.EqualTo(Errors.Unauthenticated));
    }

    [Test]
    public async Task GetFormDetailsAsync_NotFound()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        // var fakePayment = GenerateFakePaymentResponse();
        var fakeTokens = GenerateFakeTokens();

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("forms/")),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());

        var result = await client.GetFormDetailsAsync("test-slug", FormType.Event, fakeTokens);
        Assert.That(result.IsOk, Is.False);
        Assert.That(result.Error, Is.EqualTo(Errors.NotFound));
    }

    [Test]
    public async Task GetAllFormsAsync_Ok()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var fakeTokens = GenerateFakeTokens();
        var formsResponse = await ReadEmbeddedResource("GetFormsFromOrganization.json");

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("/forms")),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(formsResponse),
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());

        var request = new ListOrganizationFormsRequest();

        var result = await client.GetFormsFromOrganization(request, fakeTokens);
        Assert.That(result.IsOk, Is.True);
        var response = result.Value!;

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Data.Count, Is.EqualTo(2));
        Assert.That(response.Data[0].Title, Is.EqualTo("Test form title 1"));
        Assert.That(response.Data[1].Title, Is.EqualTo("Test form title 2"));

        Assert.That(response.Pagination.ContinuationToken, Is.EqualTo("20220921075443587"));
        Assert.That(response.Pagination.PageIndex, Is.EqualTo(1));
        Assert.That(response.Pagination.PageSize, Is.EqualTo(20));
        Assert.That(response.Pagination.TotalCount, Is.EqualTo(2));
        Assert.That(response.Pagination.TotalPages, Is.EqualTo(1));
    }

    [Test]
    public async Task GetAllFormsAsync_NotFound()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        // var fakePayment = GenerateFakePaymentResponse();
        var fakeTokens = GenerateFakeTokens();

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("/forms")),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());

        var request = new ListOrganizationFormsRequest();

        var result = await client.GetFormsFromOrganization(request, fakeTokens);
        Assert.That(result.IsOk, Is.False);
        Assert.That(result.Error, Is.EqualTo(Errors.NotFound));
    }

    [Test]
    public async Task GetAllFormsAsync_Unauthorized()
    {
        var mockLogger = new Mock<ILogger<HelloAssoClient>>();
        var mockSecretsService = new Mock<IHelloAssoSecretsService>();
        SetupSecretsService(ref mockSecretsService);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        // var fakePayment = GenerateFakePaymentResponse();
        var fakeTokens = GenerateFakeTokens();

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("/forms")),
                                              ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Forbidden));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var client = new HelloAssoClient(httpClient, mockSecretsService.Object, mockLogger.Object, GetConfiguration());

        var request = new ListOrganizationFormsRequest();

        var result = await client.GetFormsFromOrganization(request, fakeTokens);
        Assert.That(result.IsOk, Is.False);
        Assert.That(result.Error, Is.EqualTo(Errors.Unauthenticated));
    }
    #endregion
}
