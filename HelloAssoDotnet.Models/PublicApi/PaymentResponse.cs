using HelloAssoDotnet.Models.HelloAssoApi.Payment;

namespace HelloAssoDotnet.Models.PublicApi;

/// <summary>
/// Represents a payment from HelloAsso (returned object from their payments /endpoint)
/// </summary>
public record PaymentResponse
{
    /// <summary>
    /// Global payment id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Indicates the payment number (useful in the case of an order comprising payments with installments)
    /// </summary>
    public int? InstallmentNumber { get; set; }

    /// <summary>
    /// Current money transfer state. Overlaps with the Items.States but I believe it's because we have one main Payment response
    /// and many individual small payments. This one is the "master payment"
    /// </summary>
    public TransferState State { get; set; }

    /// <summary>
    /// Maps out money transfer type.
    /// </summary>
    public TransferType Type { get; set; }

    /// <summary>
    /// Offline payment means
    /// </summary>
    public PaymentMeans PaymentOfflineMeans { get; set; }

    /// <summary>
    /// Total amount of payment
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// Amount of the tips to HelloAsso, if any
    /// </summary>
    public int? AmountTip { get; set; }

    /// <summary>
    /// When this payment was registered and processed
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// What payment medium was used for this payment. (aka PaymentMethod ?)
    /// </summary>
    public PaymentMeans PaymentMeans { get; set; } = PaymentMeans.None;

    /// <summary>
    /// Placed order descriptor.
    /// </summary>
    public required Order Order { get; set; }

    /// <summary>
    /// Payer descriptor
    /// </summary>
    public required Payer Payer { get; set; }

    /// <summary>
    /// Sub-payments for this master record
    /// </summary>
    public List<PaymentRecord>? Items { get; set; }

    /// <summary>
    /// List of refund operations, if any
    /// </summary>
    public List<RefundOperation>? RefundOperations { get; set; }

    /// <summary>
    /// The date of the cash-out
    /// </summary>
    public DateTime? CashOutDate { get; set; }

    /// <summary>
    /// CashOut process state
    /// </summary>
    public CashOutState CashOutState { get; set; }

    /// <summary>
    /// Payment Receipt Url, if any
    /// </summary>
    public string? PaymentReceiptUrl { get; set; }

    /// <summary>
    /// Fiscal Receipt Url, if any
    /// </summary>
    public string? FiscalReceiptUrl { get; set; }

    /// <summary>
    /// Additional medatada
    /// </summary>
    public Dictionary<string, string> Meta { get; set; } = new();
}
