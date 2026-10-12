using DentalControl.Models.Common;

namespace DentalControl.Models.Entities;

/// <summary>
/// Representa un medicamento indicado en una receta.
/// </summary>
public class PrescriptionItem : BaseEntity
{
    public int PrescriptionId { get; set; }
    public string MedicineName { get; set; } = null!;
    public string Dosage { get; set; } = null!;
    public string Frequency { get; set; } = null!;
    public string Duration { get; set; } = null!;
    public string? AdditionalInstructions { get; set; }
    public Prescription Prescription { get; set; } = null!;
}