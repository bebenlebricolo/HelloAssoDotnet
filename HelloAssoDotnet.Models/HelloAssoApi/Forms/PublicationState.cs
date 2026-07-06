namespace HelloAssoDotnet.Models.HelloAssoApi.Forms;

/// <summary>
/// Represents the publication state of a form in the HelloAsso API.
/// </summary>
public enum PublicationState
{
    /// <summary>
    /// The form is private and only accessible through a direct link.
    /// </summary>
    Private,

    /// <summary>
    /// The form is public and accessible to everyone.
    /// </summary>
    Public,

    /// <summary>
    /// The form is in draft state and not yet published.
    /// </summary>
    Draft,

    /// <summary>
    /// The form is disabled and cannot be accessed.
    /// </summary>
    Disabled
};
