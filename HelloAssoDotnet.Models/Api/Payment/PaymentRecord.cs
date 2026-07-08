namespace HelloAssoDotnet.Models.Api.Payment;

/// <summary>
///
/// </summary>
public record PaymentRecord
{
    /// <summary>
    /// Name of this payment
    /// </summary>
    public string? Name { get; set; } = null;

    /// <summary>
    /// Amount of the payment assigned to the item and its options (in cents)
    /// </summary>
    public int ShareAmount { get; set; } = 0;

    /// <summary>
    /// Amount of the item paid on this payment term (in cents)
    /// </summary>
    public int ShareItemAmount { get; set; } = 0;

    /// <summary>
    /// Amount of all extra options linked to this item and paid on this payment (in cents)
    /// </summary>
    public int? ShareOptionsAmount { get; set; } = null;

    /// <summary>
    /// ID of the Item
    /// </summary>
    public int Id { get; set; } = 0;

    /// <summary>
    /// Total item Price in cents (after discount without extra options)
    /// </summary>
    public int Amount { get; set; } = 0;

    /// <summary>
    /// The type of payment registered
    /// </summary>
    public TierType Type { get; set; } = TierType.Payment;

    /// <summary>
    /// The raw amount (without reduction)
    /// </summary>
    public int? InitialAmount { get; set; } = null;

    /// <summary>
    /// Current payment state
    /// </summary>
    public ItemState State { get; set; } = ItemState.Unknown;
}
