using DentalControl.Models.Common;
using DentalControl.Models.Catalog;

namespace DentalControl.Models.Entities;

public class PrescriptionItem
{
    public int Id { get; set; }

    public int PrescriptionId { get; set; }

    public string MedicineName { get; set; } = string.Empty;

    public string Dosage { get; set; } = string.Empty;

    public string Frequency { get; set; } = string.Empty;

    public string Duration { get; set; } = string.Empty;

    public string? AdditionalInstructions { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public Prescription Prescription { get; set; } = null!;
}
