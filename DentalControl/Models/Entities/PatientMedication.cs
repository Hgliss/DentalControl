using DentalControl.Models.Catalog;
using DentalControl.Models.Common;

namespace DentalControl.Models.Entities;

/// <summary>
/// Representa un medicamento que toma o tomó un paciente.
/// </summary>
public class PatientMedication : BaseEntity
{
    public int PatientId { get; set; }
    public int? MedicineId { get; set; }
    public string? OtherMedicineName { get; set; }
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public string? AdministrationRoute { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Notes { get; set; }

    public Patient Patient { get; set; } = null!;
    public CatalogItem? Medicine { get; set; }
}