using DentalControl.Models.Common;

namespace DentalControl.Models.Entities;

/// <summary>
/// Representa una receta emitida en una consulta.
/// </summary>
public class Prescription : BaseEntity
{
    public int ConsultationId { get; set; }

    public DateTime PrescriptionDate { get; set; }

    public Consultation Consultation { get; set; } = null!;

    public ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
}
