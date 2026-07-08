namespace HelloAssoDotnet.Models.Api.Payment;

/// <summary>
/// Represents a payer as per the HelloAsso API documentation.
/// <see aref="https://dev.helloasso.com/reference/get_payments-paymentid"/>
/// </summary>
public record Payer
{
    /// <summary>
    /// Email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Payer's date of Birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// FirstName
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// LastName
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Physical address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// City of residence
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// ZipCode / Postal code
    /// </summary>
    public string? ZipCode { get; set; }

    /// <summary>
    /// Country of residence
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Payer's Company name
    /// </summary>
    public string? Company { get; set; }
}
