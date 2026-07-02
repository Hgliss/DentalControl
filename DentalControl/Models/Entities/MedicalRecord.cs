using DentalControl.Models.Common;
namespace DentalControl.Models.Entities;

public class MedicalRecord : AuditableEntity
{
    public int Id { get; set; }
    public string MedicalRecordCode { get; set; } = null!;
    public int PatientId { get; set; }
    public string? Allergies { get; set; }
    public string? ChronicDiseases { get; set; }
    public string? CurrentMedications { get; set; }
    public string? MedicalHistory { get; set; }
    public string? DentalHistory { get; set; }
    public string? Observations { get; set; }
    public Patient Patient { get; set; } = null!;
}
