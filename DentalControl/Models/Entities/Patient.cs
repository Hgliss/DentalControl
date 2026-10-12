using DentalControl.Models.Catalog;
using DentalControl.Models.Common;

namespace DentalControl.Models.Entities;

/// <summary>
/// Configura la tabla de pacientes, sus columnas y relaciones.
/// </summary>
public class Patient : BaseEntity
{
    public string PatientCode { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = null!;
    public string? SecondLastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public int GenderId { get; set; } 
    public string? Phone { get; set; } = null!;
    public string? Address { get; set; }
    public string? Cui { get; set; }
    public string? Occupation { get; set; }
    public string? ReferredBy { get; set; }

    public ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();

    public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();

    public ICollection<Charge> Charges { get; set; } = new List<Charge>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public ICollection<PatientHistory> Histories { get; set; } = new List<PatientHistory>();

    public ICollection<PatientAllergy> Allergies { get; set; } = new List<PatientAllergy>();

    public ICollection<PatientMedication> Medications { get; set; } = new List<PatientMedication>();

    public ICollection<PatientCondition> Conditions { get; set; } = new List<PatientCondition>();
    public CatalogItem Gender { get; set; } = null!;
}
