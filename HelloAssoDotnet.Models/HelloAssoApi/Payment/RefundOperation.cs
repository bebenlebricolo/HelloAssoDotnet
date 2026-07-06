namespace HelloAssoDotnet.Models.HelloAssoApi.Payment;

/// <summary>
/// Represents a refund operation record
/// </summary>
public record RefundOperation
{
    /// <summary>
    /// Refund operation Id
    /// </summary>
    public int Id { get; set; } = 0;

    /// <summary>
    /// Total amount to be refunded
    /// </summary>
    public int Amount { get; set; } = 0;

    /// <summary>
    /// Amount of the Tip being refunded
    /// </summary>
    public int AmountTip { get; set; } = 0;

    /// <summary>
    /// Refund status
    /// </summary>
    public RefundStatus Status { get; set; } = RefundStatus.Unknown;

    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, string> Meta { get; set; } = new Dictionary<string, string>();
}
