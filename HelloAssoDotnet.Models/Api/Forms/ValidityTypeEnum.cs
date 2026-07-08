namespace HelloAssoDotnet.Models.Api.Forms;

/// <summary>
/// Event's validity enum
/// => Represents the membership validity type
/// </summary>
public enum ValidityTypeEnum
{
    /// <summary>
    /// The validity period is based on a moving year from the subscription date
    /// </summary>
    MovingYear,

    /// <summary>
    /// The validity period is custom-defined
    /// </summary>
    Custom,

    /// <summary>
    /// The validity period is unlimited
    /// </summary>
    Illimited
}
