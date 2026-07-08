namespace HelloAssoDotnet.Models.Api.Order;

/// <summary>
/// Exposed in OrderDetails
/// </summary>
public enum PriceCategory
{
    /// <summary>
    /// Price is fixed.
    /// </summary>
    Fixed,

    /// <summary>
    /// Price is pay-what-you-want.
    /// </summary>
    Pwyw,

    /// <summary>
    /// Free
    /// </summary>
    Free
}
