using DentalControl.Models.Common;
using DentalControl.Models.Catalog;

namespace DentalControl.Models.Entities;

public class Prescription : BaseEntity
{
    public int ConsultationId { get; set; }

    public DateTime PrescriptionDate { get; set; }

    public Consultation Consultation { get; set; } = null!;

    public ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
}
