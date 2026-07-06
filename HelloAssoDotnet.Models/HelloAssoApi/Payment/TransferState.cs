namespace HelloAssoDotnet.Models.HelloAssoApi.Payment;

/// <summary>
/// Represents the Payment.State enum, it seems to overlap the <see cref="ItemState"/> but it's not the
/// same object
/// </summary>
public enum TransferState
{
    /// <summary>
    /// Payment has been abandoned by the user
    /// </summary>
    Abandoned,

    /// <summary>
    /// Payment has been authorized
    /// </summary>
    Authorized,

    /// <summary>
    /// Payment has been authorized in preprod environment
    /// </summary>
    AuthorizedPreprod,

    /// <summary>
    /// Payment has been canceled
    /// </summary>
    Canceled,

    /// <summary>
    /// Payment is being contested
    /// </summary>
    Contested,

    /// <summary>
    /// Payment has been corrected
    /// </summary>
    Corrected,

    /// <summary>
    /// Payment has been deleted
    /// </summary>
    Deleted,

    /// <summary>
    /// An error occurred during payment processing
    /// </summary>
    Error,

    /// <summary>
    /// Payment is in an inconsistent state
    /// </summary>
    Inconsistent,

    /// <summary>
    /// Payment is being initialized
    /// </summary>
    Init,

    /// <summary>
    /// No donation associated with this payment
    /// </summary>
    NoDonation,

    /// <summary>
    /// Payment is pending
    /// </summary>
    Pending,

    /// <summary>
    /// Payment has been refunded
    /// </summary>
    Refunded,

    /// <summary>
    /// Payment is being refunded
    /// </summary>
    Refunding,

    /// <summary>
    /// Payment has been refused
    /// </summary>
    Refused,

    /// <summary>
    /// Payment has been registered
    /// </summary>
    Registered,

    /// <summary>
    /// Payment state is unknown
    /// </summary>
    Unknown,

    /// <summary>
    /// Payment is waiting
    /// </summary>
    Waiting,

    /// <summary>
    /// Payment is waiting for authentication
    /// </summary>
    WaitingAuthentication,

    /// <summary>
    /// Payment is waiting for bank validation
    /// </summary>
    WaitingBankValidation,

    /// <summary>
    /// Payment is waiting for bank withdrawal
    /// </summary>
    WaitingBankWithdraw
}
