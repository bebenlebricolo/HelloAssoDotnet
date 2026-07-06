namespace HelloAssoDotnet.Models.HelloAssoApi.Payment;

/// <summary>
/// Represents the payment method used to process a transaction in the HelloAsso platform.
/// This enum defines all possible payment means that can be used by users.
/// </summary>
public enum PaymentMeans
{
    /// <summary>
    /// No payment method specified or unknown payment method.
    /// </summary>
    None,

    /// <summary>
    /// Payment made using a credit or debit card.
    /// </summary>
    Card,

    /// <summary>
    /// Payment made using SEPA (Single Euro Payments Area) bank transfer.
    /// </summary>
    Sepa,

    /// <summary>
    /// Payment made using a physical check.
    /// </summary>
    Check,

    /// <summary>
    /// Payment made using physical cash.
    /// </summary>
    Cash,

    /// <summary>
    /// Payment made using a direct bank transfer.
    /// </summary>
    BankTransfer,

    /// <summary>
    /// Payment made using any other method not listed above.
    /// </summary>
    Other
}
