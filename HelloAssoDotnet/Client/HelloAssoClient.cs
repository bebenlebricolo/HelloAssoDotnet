using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HelloAssoDotnet.Models.Configuration;
using HelloAssoDotnet.Models.HelloAssoApi.Auth;
using HelloAssoDotnet.Models.HelloAssoApi.Forms;
using HelloAssoDotnet.Models.HelloAssoApi.Order;
using HelloAssoDotnet.Models.PublicApi;
using HelloAssoDotnet.Services;
using HelloAssoDotnet.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HelloAssoDotnet.Client;

/// <summary>
/// Provides a client for interacting with the HelloAsso API, enabling operations such as
/// authentication, retrieving payment details, fetching organization forms, and downloading receipts or event tickets.
/// </summary>
public class HelloAssoClient : IHelloAssoClient
{
    private readonly HttpClient _httpClient;
    private readonly Uri _baseUri = new Uri("https://api.helloasso.com/v5");
    private const string OauthEndpoint = "https://api.helloasso.com/oauth2/token";
    private readonly ILogger<HelloAssoClient> _logger;
    private readonly IHelloAssoSecretsService _secretsService;
    private readonly AppsettingsConfiguration _appsettingsConfiguration;

    /// <summary>
    /// Used to provide user agent details and other parameters that can only be provided by the calling layer
    /// </summary>
    public ClientConfig Config { get; set; } = new ClientConfig()
    {
        UserAgent = "HelloAssoDotnetClient",
        UserAgentVersion = "1.0.0",
    };

    private string _clientId = "";
    private string _clientSecret = "";

    /// <summary>
    /// Instantiates a basic HelloAssoClient
    /// </summary>
    /// <param name="httpClient">HttpClient used under the hood</param>
    /// <param name="secretsService">Secret service is used to retrieve client id/ client secrets pair</param>
    /// <param name="logger">Logger</param>
    /// <param name="configuration">Static configuration, pulled from appsettings.</param>
    public HelloAssoClient(HttpClient httpClient,
                           IHelloAssoSecretsService secretsService,
                           ILogger<HelloAssoClient> logger,
                           IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _secretsService = secretsService;

        _appsettingsConfiguration = new AppsettingsConfiguration();
        _appsettingsConfiguration.FromConfig(configuration);
    }

    /// <summary>
    /// Retrieve HelloAsso secrets from the secrets service.
    /// </summary>
    /// <returns></returns>
    public bool RetrieveSecrets()
    {
        if (_clientId != "" && _clientSecret != "")
        {
            return true;
        }

        _clientId = _secretsService.GetClientId() ?? "";
        _clientSecret = _secretsService.GetClientSecret() ?? "";
        if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_clientSecret))
        {
            _logger.LogError("Cannot read secrets for HelloAsso client.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Appends the UserAgent header to the HttpRequestMessage
    /// </summary>
    /// <param name="request"></param>
    private void AddUserAgentHeader(ref HttpRequestMessage request)
    {
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue(Config.UserAgent, Config.UserAgentVersion));
    }

    private void AddAuthorizationHeader(ref HttpRequestMessage request, AuthTokens tokens)
    {
        request.Headers.Add("Authorization", $"Bearer {tokens.AccessToken}");
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public async Task<Result<AuthTokens>> AuthenticateAsync()
    {
        if (!RetrieveSecrets())
        {
            return Result<AuthTokens>.FromError(Errors.ClientError);
        }

        // Cache is null
        // Use the refresh token method
        var message = new HttpRequestMessage(HttpMethod.Post, OauthEndpoint);
        var payload = new Dictionary<string, string>
        {
            { "client_id", _clientId },
            { "client_secret", _clientSecret },
            { "grant_type", "client_credentials" },
        };

        message.Content = new FormUrlEncodedContent(payload);
        AddUserAgentHeader(ref message);
        var response = await _httpClient.SendAsync(message);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to authenticate to HelloAsso API");
            return Result<AuthTokens>.FromHttpResponse(response);
        }

        var content = await JsonSerializer.DeserializeAsync<AuthTokens>(await response.Content.ReadAsStreamAsync(), GetTokenJsonOptions());
        if (content == null)
        {
            return Result<AuthTokens>.FromError(Errors.ClientError);
        }
        return Result<AuthTokens>.Ok(content);
    }

    /// <summary>
    /// Custom json options for HelloAsso OAuth2 Token parsing
    /// </summary>
    /// <returns></returns>
    private static JsonSerializerOptions GetTokenJsonOptions()
    {
        return new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Refreshes the authentication tokens using a provided refresh token.
    /// </summary>
    /// <param name="tokens">The authentication tokens containing the refresh token to be used for renewing the access token.</param>
    /// <returns>A result containing the refreshed authentication tokens or an error if the operation fails.</returns>
    public async Task<Result<AuthTokens>> RefreshTokenAsync(AuthTokens tokens)
    {
        if (!RetrieveSecrets())
        {
            return Result<AuthTokens>.FromError(Errors.UnknownError);
        }

        // Use the refresh token method
        var refreshRequestMessage = new HttpRequestMessage(HttpMethod.Post, OauthEndpoint);
        var refreshPayload = new Dictionary<string, string>
        {
            { "client_id", _clientId },
            { "grant_type", "refresh_token" },
            { "refresh_token", tokens.RefreshToken },
        };
        refreshRequestMessage.Content = new FormUrlEncodedContent(refreshPayload);
        AddUserAgentHeader(ref refreshRequestMessage);
        var refreshResponse = await _httpClient.SendAsync(refreshRequestMessage);
        if (!refreshResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to authenticate to HelloAsso API");
            return Result<AuthTokens>.FromHttpResponse(refreshResponse);
        }

        // For this one, we need PascalCase Json options (which is the default)
        var refreshedToken = await JsonSerializer.DeserializeAsync<AuthTokens>(await refreshResponse.Content.ReadAsStreamAsync(), GetTokenJsonOptions());
        if (refreshedToken == null)
        {
            return new Result<AuthTokens>(Errors.ClientError);
        }
        return new Result<AuthTokens>(refreshedToken) ;
    }

    /// <summary>
    /// </summary>
    /// <param name="email"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Result<SearchPaymentResponse>> GetPaymentForUserAsync(string email, AuthTokens tokens)
    {
        string baseUrl = $"{_baseUri}/organizations/{_appsettingsConfiguration.OrganizationSlug}/payments";
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}?userSearchKey={email}&states=Authorized");
        AddAuthorizationHeader(ref requestMessage, tokens);
        AddUserAgentHeader(ref requestMessage);
        var response = await _httpClient.SendAsync(requestMessage);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Failed to get payments for user {email}");
            return Result<SearchPaymentResponse>.FromHttpResponse(response);
        }

        SearchPaymentResponse? payments = null;
        try
        {
            var jsonOptions = JsonOptionsProvider.GetJsonOptions();
            jsonOptions.RespectNullableAnnotations = true;
            payments = await JsonSerializer.DeserializeAsync<SearchPaymentResponse>(await response.Content.ReadAsStreamAsync(), jsonOptions);
        }
        catch (JsonException e)
        {
            _logger.LogError(e, "Failed to parse response.");
        }

        // Reject bad deserialization issues
        if (payments == null)
        {
            return new Result<SearchPaymentResponse>(Errors.ClientError);
        }

        return Result<SearchPaymentResponse>.Ok(payments);
    }

    /// <summary>
    /// Appends a query parameter to a URL while maintaining proper formatting.
    /// </summary>
    /// <param name="requestUrl">The initial URL to which the query parameter will be added.</param>
    /// <param name="queryData">The value of the query parameter to be appended.</param>
    /// <param name="queryName">The name of the query parameter to be appended.</param>
    /// <param name="queryCount">The number of query parameters already appended to the URL.</param>
    /// <returns>The updated URL with the appended query parameter.</returns>
    public static string AddQueryToUrl(string requestUrl, string queryData, string queryName, ref uint queryCount)
    {
        if (queryCount > 0)
        {
            requestUrl += "&";
        }
        else
        {
            requestUrl += "?";
        }
        requestUrl += $"{queryName}={queryData}";
        queryCount++;

        return requestUrl;
    }

    /// <summary>
    /// Retrieves forms associated with an organization based on the specified criteria.
    /// </summary>
    /// <param name="requestModel">The request model containing filtering parameters such as form types, page index, page size, and continuation token.</param>
    /// <param name="tokens">Authentication tokens required to access the organization's forms.</param>
    /// <returns>A task that represents the asynchronous operation, containing a result object with the list of organization forms.</returns>
    public async Task<Result<ListOrganizationFormsResponse>> GetFormsFromOrganization(ListOrganizationFormsRequest requestModel, AuthTokens tokens)
    {
        var requestUrl = $"{_baseUri}/organizations/{_appsettingsConfiguration.OrganizationSlug}/forms";

        uint queriesCount = 0;
        foreach (var formType in requestModel.FormTypes)
        {
            requestUrl = AddQueryToUrl(requestUrl, formType.ToString(), "formTypes", ref queriesCount);
        }

        if (requestModel.PageIndex != null)
        {
            requestUrl = AddQueryToUrl(requestUrl, requestModel.PageIndex.ToString()!, "pageIndex", ref queriesCount);
        }

        if (requestModel.PageSize != null)
        {
            requestUrl = AddQueryToUrl(requestUrl, requestModel.PageSize.ToString()!, "pageSize", ref queriesCount);
        }

        if (requestModel.ContinuationToken != null)
        {
            requestUrl = AddQueryToUrl(requestUrl, requestModel.ContinuationToken, "continuationToken", ref queriesCount);
        }

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        AddAuthorizationHeader(ref requestMessage, tokens);
        AddUserAgentHeader(ref requestMessage);
        var response = await _httpClient.SendAsync(requestMessage);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Failed to get forms for organization {_appsettingsConfiguration.OrganizationSlug}");
            return Result<ListOrganizationFormsResponse>.FromHttpResponse(response);
        }

        ListOrganizationFormsResponse? formsResponse = null;
        try
        {
            var jsonOptions = JsonOptionsProvider.GetJsonOptions();
            jsonOptions.RespectNullableAnnotations = true;
            formsResponse = await JsonSerializer.DeserializeAsync<ListOrganizationFormsResponse>(await response.Content.ReadAsStreamAsync(), jsonOptions);
        }
        catch (JsonException e)
        {
            _logger.LogError(e, "Failed to parse response.");
        }

        if (formsResponse == null)
        {
            return new Result<ListOrganizationFormsResponse>(Errors.ClientError);
        }
        return Result<ListOrganizationFormsResponse>.Ok(formsResponse);
    }

    /// <summary>
    /// Retrieves a single form's details based on it's slug (only "public data" will be returned)
    /// </summary>
    /// <param name="formSlug"></param>
    /// <param name="formType"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    public async Task<Result<FormDetails>> GetFormDetailsAsync(string formSlug, FormType formType, AuthTokens tokens)
    {
        string requestUrl = $"{_baseUri}/organizations/{_appsettingsConfiguration.OrganizationSlug}/forms/{formType.ToString()}/{formSlug}/public";

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        AddAuthorizationHeader(ref requestMessage, tokens);
        AddUserAgentHeader(ref requestMessage);
        var response = await _httpClient.SendAsync(requestMessage);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Failed to get form details for form {formSlug}");
            return Result<FormDetails>.FromHttpResponse(response);
        }

        FormDetails? formDetail = null;
        try
        {
            var jsonOptions = JsonOptionsProvider.GetJsonOptions();
            jsonOptions.RespectNullableAnnotations = true;
            formDetail = await JsonSerializer.DeserializeAsync<FormDetails>(await response.Content.ReadAsStreamAsync(), jsonOptions);
        }
        catch (JsonException e)
        {
            _logger.LogError(e, "Failed to parse response.");
        }

        if (formDetail == null)
        {
            return new Result<FormDetails>(Errors.ClientError);
        }
        return Result<FormDetails>.Ok(formDetail);
    }

    /// <summary>
    /// </summary>
    /// <param name="paymentId"></param>
    /// <param name="tokens"></param>
    /// <param name="withFailedRefundOperations"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Result<PaymentResponse>> GetPaymentDetailsAsync(int paymentId, AuthTokens tokens, bool withFailedRefundOperations = false)
    {
        string requestUrl = $"{_baseUri}/payments/{paymentId}";
        if (withFailedRefundOperations)
        {
            requestUrl += $"?withFailedRefundOperation=true";
        }

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        AddAuthorizationHeader(ref requestMessage, tokens);
        AddUserAgentHeader(ref requestMessage);
        var response = await _httpClient.SendAsync(requestMessage);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Failed to get payments for id {paymentId}");
            return Result<PaymentResponse>.FromHttpResponse(response);
        }

        PaymentResponse? payment = null;
        try
        {
            var jsonOptions = JsonOptionsProvider.GetJsonOptions();
            jsonOptions.RespectNullableAnnotations = true;
            payment = await JsonSerializer.DeserializeAsync<PaymentResponse>(await response.Content.ReadAsStreamAsync(), jsonOptions);
        }
        catch (JsonException e)
        {
            _logger.LogError(e, "Failed to parse response.");
        }

        if (payment == null)
        {
            return new Result<PaymentResponse>(Errors.ClientError);
        }
        return Result<PaymentResponse>.Ok(payment);
    }

    /// <summary>
    /// Retrieves the Receipt's PDF.
    /// Note that this only works when tokens have the full roles assigned (conventional Jwt tokens retrieved from API key DO NOT WORK!)
    /// </summary>
    /// <param name="payment"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    public async Task<Result<Stream>> GetPaymentReceiptPdfAsync(PaymentResponse payment, AuthTokens tokens)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, payment.PaymentReceiptUrl);

        // I noticed that's what the browser does to download the actual receipt.
        AddAuthorizationHeader(ref request, tokens);
        AddUserAgentHeader(ref request);
        request.Headers.Add("Accept", "application/pdf");
        request.Headers.Add("Upgrade-Insecure-Requests", "1");

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Failed to get receipt PDF details for order {payment.PaymentReceiptUrl}");
            _logger.LogError("Error was {error}", response.ToString());
            _logger.LogError("HttpContent was : {content}", await response.Content.ReadAsStringAsync());
            return Result<Stream>.FromHttpResponse(response);
        }

        return  Result<Stream>.Ok(await response.Content.ReadAsStreamAsync());
    }

    /// <summary>
    /// Retrieves the PDFs of event tickets associated with a given payment.
    /// </summary>
    /// <param name="payment">The payment response containing details of the associated order.</param>
    /// <param name="tokens">Authentication tokens required to access secured resources.</param>
    /// <returns>A result containing a list of streams representing ticket PDFs, or an error if the retrieval fails.</returns>
    public async Task<Result<List<Stream>>> GetEventTicketPdf(PaymentResponse payment, AuthTokens tokens)
    {
        var orderDetails = await GetOrderDetailsAsync(payment.Order.Id, tokens);
        if (!orderDetails.IsOk)
        {
            _logger.LogError($"Failed to get order details for order {payment.Order.Id}");
            _logger.LogError("Error was {error}", orderDetails.Error);
            return Result<List<Stream>>.FromError(orderDetails.Error);
        }

        List<string> ticketsUrls = orderDetails.Value!.Items!.Select(ticket => ticket.TicketUrl).ToList()!;
        List<Stream> ticketsPdfs = new List<Stream>();

        List<Task<HttpResponseMessage>> tasks = new List<Task<HttpResponseMessage>>();
        foreach (var ticketUrl in ticketsUrls)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, ticketUrl);

            // I noticed that's what the browser does to download the actual receipt.
            AddAuthorizationHeader(ref request, tokens);
            AddUserAgentHeader(ref request);
            request.Headers.Add("Accept", "application/pdf");
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            var task = _httpClient.SendAsync(request);
            tasks.Add(task);
        }

        // Wait all tasks
        await Task.WhenAll(tasks).ConfigureAwait(false);
        var errors = new Dictionary<string, HttpResponseMessage>();
        for (int i = 0 ; i < tasks.Count ; i++)
        {
            var result = tasks[i].Result;
            if (!result.IsSuccessStatusCode)
            {
                errors.Add(ticketsUrls[i], result);
            }
            else
            {
                ticketsPdfs.Add(await result.Content.ReadAsStreamAsync());
            }
        }

        if (errors.Count > 0)
        {
            _logger.LogError("Encountered issues while downloading tickets:");
            foreach (var error in errors)
            {
                _logger.LogError("For ticket : {ticket}",error.Key);
                _logger.LogError(error.Value.ToString());
                _logger.LogError("HttpContent was : {content}", await error.Value.Content.ReadAsStringAsync());
            }

            return Result<List<Stream>>.FromError(Errors.ServerError);
        }
        return  Result<List<Stream>>.Ok(ticketsPdfs);
    }

    /// <summary>
    /// Retrieves the details of an order based on the provided order ID.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order to fetch details for.</param>
    /// <param name="tokens">Authentication tokens needed to authorize the request.</param>
    /// <returns>A result containing the order details if the operation succeeds, or an error if it fails.</returns>
    public async Task<Result<OrderDetails>> GetOrderDetailsAsync(int orderId, AuthTokens tokens)
    {
        var endpointUrl = $"{_baseUri}/orders/{orderId}";
        var request = new HttpRequestMessage(HttpMethod.Get, endpointUrl);
        request.Headers.Add("Accept", "application/json");
        AddAuthorizationHeader(ref request, tokens);
        AddUserAgentHeader(ref request);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return Result<OrderDetails>.FromHttpResponse(response);
        }

        OrderDetails? order = null;
        try
        {
            order = await JsonSerializer.DeserializeAsync<OrderDetails>(await response.Content.ReadAsStreamAsync(), JsonOptionsProvider.GetJsonOptions());
            if (order == null)
            {
                return new Result<OrderDetails>(Errors.NotFound);
            }
        }
        catch (JsonException e)
        {
            _logger.LogError(e, "Failed to parse response.");
            return Result<OrderDetails>.FromHttpResponse(response);
        }

        return  Result<OrderDetails>.Ok(order!);
    }
}
