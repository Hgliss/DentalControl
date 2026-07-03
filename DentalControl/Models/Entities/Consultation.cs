using DentalControl.Models.Catalog;
using DentalControl.Models.Common;

namespace DentalControl.Models.Entities;

public class Consultation : BaseEntity
{
    public int PatientId { get; set; }

    public int? TreatmentId { get; set; }

    public int ConsultationTypeId { get; set; }

    public DateTime ConsultationDate { get; set; }

    public string? BloodPressure { get; set; }

    public string? HeartRate { get; set; }

    public decimal? OxygenSaturation { get; set; }

    public decimal? Temperature { get; set; }

    public string? PhysicalNotes { get; set; }

    public string ProcedureDescription { get; set; } = string.Empty;

    public string? MedicalObservations { get; set; }

    public bool RequiresFollowUp { get; set; }

    public decimal ConsultationCost { get; set; }

    public Patient Patient { get; set; } = null!;

    public Treatment? Treatment { get; set; }

    public ConsultationType ConsultationType { get; set; } = null!;

    public Prescription? Prescription { get; set; }
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public ICollection<Charge> Charges { get; set; } = new List<Charge>();
}