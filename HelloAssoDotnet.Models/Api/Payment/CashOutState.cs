namespace HelloAssoDotnet.Models.Api.Payment;

/// <summary>
/// CashOut current processing state.
/// From HellAsso documentation
/// <see aref="https://dev.helloasso.com/reference/get_payments-paymentid"/>
/// </summary>
public enum CashOutState
{
    /// <summary>
    /// Represents the state where money has been received or is in the system
    /// but has not yet been transferred or processed further.
    /// </summary>
    MoneyIn,

    /// <summary>
    /// Indicates that the transfer cannot be completed because the receiver's account is full.
    /// </summary>
    CantTransferReceiverFull,

    /// <summary>
    /// The money has been successfully transferred.
    /// </summary>
    Transfered,

    /// <summary>
    /// The payment has been refunded to the original payer.
    /// </summary>
    Refunded,

    /// <summary>
    /// A refund is currently in progress.
    /// </summary>
    Refunding,

    /// <summary>
    /// The system is waiting for confirmation before proceeding with the cash out.
    /// </summary>
    WaitingForCashOutConfirmation,

    /// <summary>
    /// The money has been successfully cashed out from the system.
    /// </summary>
    CashedOut,

    /// <summary>
    /// The state of the cash out is unknown or cannot be determined.
    /// </summary>
    Unknown,

    /// <summary>
    /// The payment or transfer is being contested or disputed.
    /// </summary>
    Contested,

    /// <summary>
    /// A transfer is currently in progress.
    /// </summary>
    TransferInProgress
}
