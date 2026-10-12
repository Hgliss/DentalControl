using DentalControl.Models.Catalog;
using DentalControl.Models.Common;

namespace DentalControl.Models.Entities;

/// <summary>
/// Representa un antecedente médico u odontológico de un paciente.
/// </summary>
public class PatientHistory : BaseEntity
{
    public int PatientId { get; set; }
    public int HistoryTypeId { get; set; }
    public string Description { get; set; } = null!;
    public DateOnly? EventDate { get; set; }
    public string? Notes { get; set; }
    public Patient Patient { get; set; } = null!;
    public CatalogItem HistoryType { get; set; } = null!;
}
