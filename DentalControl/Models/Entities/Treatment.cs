using DentalControl.Models.Catalog;
using DentalControl.Models.Common;

namespace DentalControl.Models.Entities;

/// <summary>
/// Representa un tratamiento de un paciente.
/// </summary>
public class Treatment : BaseEntity
{
    public int PatientId { get; set; }
    public string TreatmentType { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal TotalCost { get; set; }
    public int? EstimatedSessionCount { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int TreatmentStatusId { get; set; }
    public string? ProgressNotes { get; set; }

    public Patient Patient { get; set; } = null!;
    public CatalogItem TreatmentStatus { get; set; } = null!;

    public ICollection<Charge> Charges { get; set; } = new List<Charge>();
    public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
}
