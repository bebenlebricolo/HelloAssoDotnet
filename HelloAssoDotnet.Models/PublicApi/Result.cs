namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Used to wrap output response in an Optional kind of result type.
/// If Result is null, then check <see cref="Error"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public record Result<T> where T : class
{
    /// <summary>
    /// Standard result constructor.
    /// </summary>
    /// <param name="value"></param>
    public Result(T value)
    {
        Value = value;
        Error = Errors.Ok;
    }

    /// <summary>
    /// Error result constructor.
    /// </summary>
    /// <param name="error"></param>
    public Result(Errors error)
    {
        Value = null;
        Error = error;
    }

    /// <summary>
    /// Shorthand Ok object return kind
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Result<T> Ok(T value)
    {
        return new Result<T>(value);
    }

    /// <summary>
    /// Convert from a HttpResponseMessage to internal error model
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    public static Result<T> FromHttpResponse(HttpResponseMessage response)
    {
        var error = ErrorModelConverter.FromHttpResponse(response);
        return new Result<T>(error);
    }

    /// <summary>
    /// Shorthand Error object
    /// </summary>
    /// <returns></returns>
    public static Result<T> FromError(Errors error)
    {
        return new Result<T>(error);
    }

    /// <summary>
    /// Shorthand to know if this Result object is safe to use.
    /// </summary>
    public bool IsOk => Error == Errors.Ok && Value != null;

    /// <summary>
    /// Nullable result payload.
    /// </summary>
    public T? Value { get; } = null;

    /// <summary>
    /// Internal error flag
    /// </summary>
    public Errors Error { get; set; } = Errors.Ok;
}
