namespace HelloAssoDotnet.Models.HelloAssoApi.Payment;

/// <summary>
/// Represents Money transfer type.
/// </summary>
public enum TransferType
{
    /// <summary>
    /// Represents an offline payment (cash, check, etc.)
    /// </summary>
    Offline,

    /// <summary>
    /// Represents a credit payment (money received)
    /// </summary>
    Credit,

    /// <summary>
    /// Represents a debit payment (money spent)
    /// </summary>
    Debit
}
