namespace HelloAssoDotnet.Models.Api.Payment;

/// <summary>
/// Lists available payment types for a PaymentRecord
/// <see cref="PaymentRecord"/>
/// </summary>
public enum TierType
{
    /// <summary>
    ///
    /// </summary>
    Donation,

    /// <summary>
    ///
    /// </summary>
    Payment,

    /// <summary>
    ///
    /// </summary>
    Registration,

    /// <summary>
    ///
    /// </summary>
    Membership,

    /// <summary>
    ///
    /// </summary>
    MonthlyDonation,

    /// <summary>
    ///
    /// </summary>
    MonthlyPayment,

    /// <summary>
    ///
    /// </summary>
    OfflineDonation,

    /// <summary>
    ///
    /// </summary>
    Contribution,

    /// <summary>
    ///
    /// </summary>
    Bonus,

    /// <summary>
    ///
    /// </summary>
    Product
}
