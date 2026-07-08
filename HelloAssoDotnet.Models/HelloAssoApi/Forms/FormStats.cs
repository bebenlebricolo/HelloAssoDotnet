namespace HelloAssoDotnet.Models.HelloAssoApi.Forms;

/// <summary>
/// Aggregated statistics for a form.
/// <see aref="https://dev.helloasso.com/reference/get_organizations-organizationslug-forms-formtype-formslug-stats"/>
/// </summary>
public record FormStats
{
    /// <summary>
    /// Total collected amount for the form, in cents.
    /// </summary>
    public int AmountCollected { get; set; }

    /// <summary>
    /// Total number of orders placed on the form.
    /// </summary>
    public int OrderCount { get; set; }

    /// <summary>
    /// Total number of payments processed on the form.
    /// </summary>
    public int PaymentCount { get; set; }

    /// <summary>
    /// Number of distinct contributors.
    /// </summary>
    public int ContributorCount { get; set; }
}
