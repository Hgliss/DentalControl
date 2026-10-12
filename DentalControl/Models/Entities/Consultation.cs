using DentalControl.Models.Common;

namespace DentalControl.Models.Entities;

/// <summary>
/// Representa una consulta clínica de un paciente.
/// </summary>
public class Consultation : BaseEntity
{
    public int PatientId { get; set; }
    public int? TreatmentId { get; set; }
    public DateTime ConsultationDate { get; set; }
    public string ConsultationType { get; set; } = null!;
    public string ReasonForVisit { get; set; } = null!;

    public int? SystolicPressure { get; set; }
    public int? DiastolicPressure { get; set; }
    public int? HeartRate { get; set; }
    public decimal? OxygenSaturation { get; set; }
    public decimal? Temperature { get; set; }

    public string? PhysicalNotes { get; set; }
    public string? DiagnosisDescription { get; set; }
    public string? ProcedureDescription { get; set; }
    public string? MedicalObservations { get; set; }

    public bool RequiresFollowUp { get; set; }
    public DateTime? NextAppointment { get; set; }
    public decimal ConsultationCost { get; set; }

    public Patient Patient { get; set; } = null!;
    public Treatment? Treatment { get; set; }

    public ICollection<Charge> Charges { get; set; } = new List<Charge>();
}