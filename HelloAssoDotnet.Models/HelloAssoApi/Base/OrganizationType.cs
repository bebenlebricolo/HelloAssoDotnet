using System.Text.Json.Serialization;

namespace HelloAssoDotnet.Models.HelloAssoApi.Base;

/// <summary>
/// Records all available organization types in HelloAsso system
/// <see aref="https://dev.helloasso.com/reference/get_payments-paymentid"/>
/// </summary>
public enum OrganizationType
{
    /// <summary>
    ///
    /// </summary>
    Association1901Rig,

    /// <summary>
    ///
    /// </summary>
    Association1901Rup,

    /// <summary>
    ///
    /// </summary>
    Association1901,

    /// <summary>
    ///
    /// </summary>
    FondationRup,

    /// <summary>
    ///
    /// </summary>
    FondDotation,

    /// <summary>
    ///
    /// </summary>
    FondationSousEgide,

    /// <summary>
    ///
    /// </summary>
    FondationScientifique,

    /// <summary>
    ///
    /// </summary>
    FondationPartenariale,

    /// <summary>
    ///
    /// </summary>
    FondationUniversitaire,

    /// <summary>
    ///
    /// </summary>
    FondationHospitaliere,

    /// <summary>
    ///
    /// </summary>
    Association1905,

    /// <summary>
    ///
    /// </summary>
    Association1905Rup,

    /// <summary>
    ///
    /// </summary>
    Entreprise,

    /// <summary>
    ///
    /// </summary>
    Cooperative,

    /// <summary>
    ///
    /// </summary>
    Etablissement,

    /// <summary>
    ///
    /// </summary>
    Association1908,

    /// <summary>
    ///
    /// </summary>
    Association1908Rig,

    /// <summary>
    ///
    /// </summary>
    Association1908Rup,

    /// <summary>
    ///
    /// </summary>
    AssociationMilitaire,

    /// <summary>
    ///
    /// </summary>
    AssociationProprietaire,

    /// <summary>
    ///
    /// </summary>
    Collectivités,

    /// <summary>
    ///
    /// </summary>
    ComiteEntreprise,

    /// <summary>
    ///
    /// </summary>
    [JsonPropertyName("CSE")]
    CSE,

    /// <summary>
    ///
    /// </summary>
    FabriqueEglise,

    /// <summary>
    ///
    /// </summary>
    FondsPerenite,

    /// <summary>
    ///
    /// </summary>
    [JsonPropertyName("GIE")]
    GIE,

    /// <summary>
    ///
    /// </summary>
    [JsonPropertyName("GIP")]
    GIP,

    /// <summary>
    ///
    /// </summary>
    MenseCuriale,

    /// <summary>
    ///
    /// </summary>
    [JsonPropertyName("SCIC")]
    SCIC,

    /// <summary>
    ///
    /// </summary>
    [JsonPropertyName("SCOP")]
    SCOP,

    /// <summary>
    ///
    /// </summary>
    Autres
}
