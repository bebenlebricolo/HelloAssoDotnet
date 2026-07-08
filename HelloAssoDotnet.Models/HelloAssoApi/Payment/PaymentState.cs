namespace HelloAssoDotnet.Models.HelloAssoApi.Payment;

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
    Waiting
}
