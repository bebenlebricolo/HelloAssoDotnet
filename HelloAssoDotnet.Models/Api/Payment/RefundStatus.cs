namespace HelloAssoDotnet.Models.Api.Payment;

/// <summary>
/// Refund process status
/// </summary>
public enum RefundStatus
{
    /// <summary>
    ///
    /// </summary>
    Unknown,

    /// <summary>
    ///
    /// </summary>
    Init,

    /// <summary>
    ///
    /// </summary>
    Processing,

    /// <summary>
    ///
    /// </summary>
    Processed,

    /// <summary>
    ///
    /// </summary>
    Error,

    /// <summary>
    ///
    /// </summary>
    InternalError,

    /// <summary>
    ///
    /// </summary>
    Refused,

    /// <summary>
    ///
    /// </summary>
    Aborted,

    /// <summary>
    ///
    /// </summary>
    Canceled
}
