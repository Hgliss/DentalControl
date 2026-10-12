using DentalControl.Models.Catalog;
using DentalControl.Models.Common;

namespace DentalControl.Models.Entities;

/// <summary>
/// Representa un padecimiento de un paciente.
/// </summary>
public class PatientCondition : BaseEntity
{
    public int PatientId { get; set; }
    public int? ConditionId { get; set; }
    public string? OtherConditionName { get; set; }
    public DateOnly? DiagnosisDate { get; set; }
    public int? ClinicalStatusId { get; set; }
    public string? Notes { get; set; }

    public Patient Patient { get; set; } = null!;
    public CatalogItem? Condition { get; set; }
    public CatalogItem? ClinicalStatus { get; set; }
}