namespace HelloAssoDotnet.Models.Api.Payment;

/// <summary>
/// Payment states used to filter payment listings.
/// <see aref="https://dev.helloasso.com/docs/etats-des-paiements"/>
/// </summary>
public enum PaymentState
{
    /// <summary>
    /// A payment is waiting to be processed (e.g. bank transfer or check not received yet).
    /// </summary>
    Pending,

    /// <summary>
    /// Payment has been authorized/captured successfully.
    /// </summary>
    Authorized,

    /// <summary>
    /// Payment has been refused.
    /// </summary>
    Refused,

    /// <summary>
    /// State could not be determined.
    /// </summary>
    Unknown,

    /// <summary>
    /// Payment has been registered (e.g. offline means recorded manually).
    /// </summary>
    Registered,

    /// <summary>
    /// Payment has been refunded.
    /// </summary>
    Refunded,

    /// <summary>
    /// Payment is in the process of being refunded.
    /// </summary>
    Refunding,

    /// <summary>
    /// Payment is being contested (chargeback).
    /// </summary>
    Contested,

    /// <summary>
    /// Payment is waiting (installment or SEPA processing).
    /// </summary>
    Waiting,

    /// <summary>
    /// Payment failed due to an error.
    /// </summary>
    Error,

    /// <summary>
    /// Payment has been canceled.
    /// </summary>
    Canceled,

    /// <summary>
    /// Payment is waiting for bank validation.
    /// </summary>
    WaitingBankValidation,

    /// <summary>
    /// Payment is waiting for a bank withdrawal.
    /// </summary>
    WaitingBankWithdraw,

    /// <summary>
    /// Payment has been abandoned by the payer.
    /// </summary>
    Abandoned,

    /// <summary>
    /// Payment is waiting for a strong authentication (3-D Secure).
    /// </summary>
    WaitingAuthentication,

    /// <summary>
    /// Payment has been authorized on the pre-production environment.
    /// </summary>
    AuthorizedPreprod,

    /// <summary>
    /// Payment has been corrected.
    /// </summary>
    Corrected,

    /// <summary>
    /// Payment has been deleted.
    /// </summary>
    Deleted,

    /// <summary>
    /// Payment is in an inconsistent state.
    /// </summary>
    Inconsistent,

    /// <summary>
    /// No donation was made.
    /// </summary>
    NoDonation,

    /// <summary>
    /// Payment has just been initialized.
    /// </summary>
    Init
}
