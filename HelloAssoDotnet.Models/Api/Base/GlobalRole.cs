namespace HelloAssoDotnet.Models.Api.Base;

/// <summary>
/// Role held on a resource (organization or form) by the caller.
/// <see aref="HelloAsso.Models.Enums.GlobalRole"/>
/// </summary>
public enum GlobalRole
{
    /// <summary>
    /// Administrator of the whole organization.
    /// </summary>
    OrganizationAdmin,

    /// <summary>
    /// Administrator of a single form.
    /// </summary>
    FormAdmin
}
