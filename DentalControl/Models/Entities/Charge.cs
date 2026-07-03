using DentalControl.Models.Common;
using DentalControl.Models.Catalog;

namespace DentalControl.Models.Entities;

public class Charge : BaseEntity
{
    public int PatientId { get; set; }

    public int? ConsultationId { get; set; }

    public int? TreatmentId { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime ChargeDate { get; set; }

    public Patient Patient { get; set; } = null!;

    public Consultation? Consultation { get; set; }

    public Treatment? Treatment { get; set; }

    public ICollection<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();

}
