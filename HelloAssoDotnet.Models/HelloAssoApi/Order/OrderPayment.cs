using HelloAssoDotnet.Models.HelloAssoApi.Payment;

namespace HelloAssoDotnet.Models.HelloAssoApi.Order;

/// <summary>
/// Share item
/// </summary>
public record ShareItem
{
    /// <summary>
    /// Id of the order item
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Amount of the payment assigned to the item and its options (in cents)
    /// </summary>
    public int ShareAmount { get; set; }

    /// <summary>
    /// Amount of the item paid on this payment term (in cents)
    /// </summary>
    public int ShareItemAmount { get; set; }

    /// <summary>
    /// Amount of all extra options linked to this item and paid on this payment (in cents)
    /// </summary>
    public int? ShareOptionsAmount { get; set; } = null;
}

/// <summary>
/// Really looks like <see cref="Payment.PaymentRecord"/> but in a slightly simpler form...
/// </summary>
public record OrderPayment
{
    /// <summary>
    /// Items linked to this payment and each share between the item and the payment
    /// </summary>
    public List<ShareItem>? Items { get; set; } = null;

    /// <summary>
    /// The date of the cash-out
    /// </summary>
    public DateTime? CashOutDate { get; set; }

    /// <summary>
    /// Current cash-out state
    /// </summary>
    public CashOutState CashOutState { get; set; } = CashOutState.Unknown;

    /// <summary>
    /// The Payment Receipt Url
    /// </summary>
    public string? PaymentReceiptUrl { get; set; } = null;

    /// <summary>
    /// The Fiscal Receipt Url
    /// </summary>
    public string? FiscalReceiptUrl { get; set; } = null;

    /// <summary>
    /// The ID of the payment
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Total Amount of the payment (in cents)
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// Tip Amount of the payment (in cents)
    /// </summary>
    public int? AmountTip { get; set; } = null;

    /// <summary>
    /// Date of the payment
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Payment means
    /// </summary>
    public PaymentMeans PaymentMeans { get; set; } = PaymentMeans.Card;

    /// <summary>
    /// Indicates the payment number (useful in the case of an order comprising payments with installments)
    /// </summary>
    public int? InstallmentNumber { get; set; } = null;

    /// <summary>
    /// Transfer state
    /// </summary>
    public TransferState State { get; set; } = TransferState.Unknown;

    /// <summary>
    /// Type of money transfer
    /// </summary>
    public TransferType Type { get; set; } = TransferType.Debit;

    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, string> Meta  { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Offline payment mean
    /// </summary>
    public PaymentMeans PaymentOfflineMean { get; set; } = PaymentMeans.None;

    /// <summary>
    /// The refund operations information for the specific payment.
    /// </summary>
    public List<RefundOperation>? RefundOperations { get; set; } = null;
}
