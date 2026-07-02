using DentalControl.Models.Common;
using DentalControl.Models.Catalog;

namespace DentalControl.Models.Entities;

public class Treatment : AuditableEntity
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int TreatmentTypeId { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal TotalCost { get; set; }

    public int? SessionCount { get; set; }

    public string? ProgressNotes { get; set; }

    public Patient Patient { get; set; } = null!;

    public TreatmentType TreatmentType { get; set; } = null!;

    public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Charge> Charges { get; set; } = new List<Charge>();
}
