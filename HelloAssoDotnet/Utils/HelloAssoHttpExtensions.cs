using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HelloAssoDotnet.Models.Configuration;
using HelloAssoDotnet.Models.PublicApi;
using Microsoft.Extensions.Logging;

namespace HelloAssoDotnet.Utils;

/// <summary>
/// Small, stateless helpers shared by every sub-client. These are plain functions used to remove the
/// repetitive request-building / response-parsing boilerplate. They deliberately do NOT own the
/// <see cref="HttpClient"/> nor any state: each sub-client keeps using its own HttpClient directly.
/// </summary>
public static class HelloAssoHttpExtensions
{
    /// <summary>
    /// Adds the <c>Authorization: Bearer {token}</c> header (kept identical to the historical behavior).
    /// </summary>
    /// <param name="request">Request being built</param>
    /// <param name="accessToken">Bearer access token</param>
    /// <returns>The same request, for chaining.</returns>
    public static HttpRequestMessage WithBearer(this HttpRequestMessage request, string accessToken)
    {
        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        return request;
    }

    /// <summary>
    /// Appends the configured User-Agent header.
    /// </summary>
    /// <param name="request">Request being built</param>
    /// <param name="config">Client configuration holding the user agent details</param>
    /// <returns>The same request, for chaining.</returns>
    public static HttpRequestMessage WithUserAgent(this HttpRequestMessage request, ClientConfig config)
    {
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue(config.UserAgent, config.UserAgentVersion));
        return request;
    }

    /// <summary>
    /// Sets <c>Accept: application/json</c>.
    /// </summary>
    /// <param name="request">Request being built</param>
    /// <returns>The same request, for chaining.</returns>
    public static HttpRequestMessage WithJsonAccept(this HttpRequestMessage request)
    {
        request.Headers.Add("Accept", "application/json");
        return request;
    }

    /// <summary>
    /// Mimics the browser headers used to download a PDF asset (receipts, tickets).
    /// </summary>
    /// <param name="request">Request being built</param>
    /// <returns>The same request, for chaining.</returns>
    public static HttpRequestMessage WithPdfAccept(this HttpRequestMessage request)
    {
        // I noticed that's what the browser does to download the actual receipt.
        request.Headers.Add("Accept", "application/pdf");
        request.Headers.Add("Upgrade-Insecure-Requests", "1");
        return request;
    }

    /// <summary>
    /// Serializes <paramref name="body"/> as a JSON request body (camelCase, matching the API).
    /// </summary>
    /// <typeparam name="TBody">Body type</typeparam>
    /// <param name="request">Request being built</param>
    /// <param name="body">Object serialized as the request body</param>
    /// <returns>The same request, for chaining.</returns>
    public static HttpRequestMessage WithJsonBody<TBody>(this HttpRequestMessage request, TBody body)
    {
        var json = JsonSerializer.Serialize(body, JsonOptionsProvider.GetJsonOptions());
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        return request;
    }

    /// <summary>
    /// Sends the request and deserializes a successful JSON body into a <see cref="Result{T}"/>.
    /// On a non-success status, the HTTP status is mapped to the internal error model.
    /// </summary>
    /// <typeparam name="T">Expected payload type</typeparam>
    /// <param name="httpClient">HttpClient owned by the calling sub-client</param>
    /// <param name="request">Fully built request (headers already set)</param>
    /// <param name="logger">Logger used to report parsing issues</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A result wrapping the payload or the mapped error.</returns>
    public static async Task<Result<T>> SendJsonAsync<T>(this HttpClient httpClient,
                                                         HttpRequestMessage request,
                                                         ILogger logger,
                                                         CancellationToken cancellationToken = default) where T : class
    {
        var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return Result<T>.FromHttpResponse(response);
        }

        return await response.DeserializeResultAsync<T>(logger, cancellationToken);
    }

    /// <summary>
    /// Deserializes the (successful) response body into a <see cref="Result{T}"/>.
    /// Parsing issues are logged and surfaced as <see cref="Errors.ClientError"/>.
    /// </summary>
    /// <typeparam name="T">Expected payload type</typeparam>
    /// <param name="response">Response whose body must be parsed</param>
    /// <param name="logger">Logger used to report parsing issues</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A result wrapping the payload or a client error.</returns>
    public static async Task<Result<T>> DeserializeResultAsync<T>(this HttpResponseMessage response,
                                                                  ILogger logger,
                                                                  CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var jsonOptions = JsonOptionsProvider.GetJsonOptions();
            jsonOptions.RespectNullableAnnotations = true;
            var value = await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(cancellationToken), jsonOptions, cancellationToken);
            if (value == null)
            {
                return new Result<T>(Errors.ClientError);
            }
            return Result<T>.Ok(value);
        }
        catch (JsonException e)
        {
            logger.LogError(e, "Failed to parse response.");
            return new Result<T>(Errors.ClientError);
        }
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
}
