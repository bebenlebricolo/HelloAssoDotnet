namespace HelloAssoDotnet.Models.Api.Forms;

/// <summary>
/// Place datamodel, used to locate an event (linked to forms) on the Map
/// </summary>
public record Place
{
    /// <summary>
    /// Place's address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Shortname of the place
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// City's name
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Zip/Postal code
    /// </summary>
    public string? ZipCode { get; set; }

    /// <summary>
    /// Country
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Place's exact geolocation
    /// </summary>
    public Geolocation? Geolocation { get; set; }
}