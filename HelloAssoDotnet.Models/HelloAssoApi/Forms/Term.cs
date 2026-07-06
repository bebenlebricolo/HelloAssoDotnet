namespace HelloAssoDotnet.Models.HelloAssoApi.Forms;

/// <summary>
/// Represents a term in a HelloAsso form. A term typically includes
/// information about a specific amount and the related date.
/// </summary>
public record Term
{
    /// <summary>
    /// Term date
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// Term amount
    /// </summary>
    public int? Amount { get; set; }
}
