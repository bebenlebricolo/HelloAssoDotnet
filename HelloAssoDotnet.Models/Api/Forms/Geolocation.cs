namespace HelloAssoDotnet.Models.Api.Forms;

/// <summary>
/// Geolocation using common GPS coordinates
/// </summary>
public record Geolocation
{
    /// <summary>
    /// Geolocation latitude
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Geolocation Longitude
    /// </summary>
    public double Longitude { get; set; }
}