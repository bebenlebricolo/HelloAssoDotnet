using System.Text.Json.Serialization;

namespace HelloAssoDotnet.Models.Api.Forms;

/// <summary>
/// Currencies
/// </summary>
public enum CurrencyEnum
{
    /// <summary>
    /// Us Dollars
    /// </summary>
    [JsonStringEnumMemberName("USD")]
    Usd,

    /// <summary>
    /// Euro
    /// </summary>
    [JsonStringEnumMemberName("EUR")]
    Euro,

    /// <summary>
    /// Fallback value
    /// </summary>
    Unknown
}
