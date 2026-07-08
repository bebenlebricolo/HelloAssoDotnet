namespace HelloAssoDotnet.Models.Api.Forms;

/// <summary>
/// Represents all available form types from HelloAsso Api.
/// Example <see aref="https://dev.helloasso.com/reference/get_payments-paymentid"/>
/// </summary>
public enum FormType
{
    /// <summary>
    /// Payment for an Event form
    /// </summary>
    Event,

    /// <summary>
    /// Payment for a CrowdFunding event
    /// </summary>
    CrowdFunding,

    /// <summary>
    /// Regular membership payment
    /// </summary>
    Membership,

    /// <summary>
    /// Unilateral donation
    /// </summary>
    Donation,

    /// <summary>
    /// Basic Payment form
    /// </summary>
    PaymentForm,

    /// <summary>
    /// Shopping Checkout
    /// </summary>
    Checkout,

    /// <summary>
    /// Regular Shop payment
    /// </summary>
    Shop
}
