using DentalControl.Models.Catalog;
using DentalControl.Models.Common;

namespace DentalControl.Models.Entities;

/// <summary>
/// Representa una alergia de un paciente.
/// </summary>
public class PatientAllergy : BaseEntity
{
    public int PatientId { get; set; }
    public int? AllergenId { get; set; }
    public string? OtherAllergenName { get; set; }
    public string? Reaction { get; set; }
    public string? Notes { get; set; }

    public Patient Patient { get; set; } = null!;
    public CatalogItem? Allergen { get; set; }
}
