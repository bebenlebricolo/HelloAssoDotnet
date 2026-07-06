using System.Net;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Lighter error model based on HttpResponse codes
/// </summary>
public enum Errors
{
    /// <summary>
    /// Any sort of success statuses
    /// </summary>
    Ok,

    /// <summary>
    /// Targeted item not found
    /// </summary>
    NotFound,

    /// <summary>
    /// Standard bad request
    /// </summary>
    BadRequest,

    /// <summary>
    /// Call is not authenticated nor authorized.
    /// Token can be wrong, expired, etc...
    /// </summary>
    Unauthenticated,

    /// <summary>
    /// General remote service unavailability, internal errors, etc.
    /// </summary>
    ServerError,

    /// <summary>
    /// Wraps an internal client-side error (could be serialization issues, missing files, null-refs, etc... happening within
    /// the boundaries of this library, and not directly caused by any external calls.)
    /// </summary>
    ClientError,

    /// <summary>
    /// No clear identification. Check logs.
    /// </summary>
    UnknownError
}

/// <summary>
/// ErrorModelConverter works with <see cref="Errors"/> and <see cref="HttpResponseMessage"/>
/// </summary>
public static class ErrorModelConverter
{
    /// <summary>
    /// Converts an HttpResponseMessage to the Api error model
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    public static Errors FromHttpResponse(HttpResponseMessage response)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                return Errors.Ok;

            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.Forbidden:
                return Errors.Unauthenticated;

            case HttpStatusCode.ServiceUnavailable:
            case HttpStatusCode.InternalServerError:
                return Errors.ServerError;

            case HttpStatusCode.BadRequest:
                return Errors.BadRequest;

            case HttpStatusCode.NotFound:
                return Errors.NotFound;

            default:
                return Errors.UnknownError;
        }
    }

    /// <summary>
    /// Try to convert from the error model to approximately equivalent http status code.
    /// There is an approximation as ErrorModel is considerably simpler than the whole HttpStatusCode spectrum.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static HttpStatusCode ToHttpStatus(Errors error)
    {
        switch (error)
        {
            case Errors.Ok:
                return HttpStatusCode.OK;

            case Errors.BadRequest:
                return HttpStatusCode.BadRequest;

            case Errors.NotFound:
                return HttpStatusCode.NotFound;

            case Errors.ServerError:
                return HttpStatusCode.InternalServerError;

            // Aggregation of the 2 states (unauthorized and forbidden)
            case Errors.Unauthenticated:
                return HttpStatusCode.Unauthorized;

            // Not quite symmetric, but I don't think that's a use case we'll meet very often
            case Errors.UnknownError:
            default:
                return (HttpStatusCode) 418; // I'm a teapot !
        }
    }
}
