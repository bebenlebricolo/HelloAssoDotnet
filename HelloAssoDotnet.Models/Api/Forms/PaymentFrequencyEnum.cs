namespace HelloAssoDotnet.Models.Api.Forms;

/// <summary>
/// Represents the frequency at which payments can be made in HelloAsso forms.
/// </summary>
public enum PaymentFrequencyEnum
{
    /// <summary>
    /// Single one-time payment.
    /// </summary>
    Single,

    /// <summary>
    /// Payment divided into fixed installments.
    /// </summary>
    Installment,

    /// <summary>
    /// Recurring monthly payment.
    /// </summary>
    Monthly
}
