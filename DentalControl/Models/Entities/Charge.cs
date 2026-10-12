using DentalControl.Models.Common;

namespace DentalControl.Models.Entities;

/// <summary>
/// Representa un cargo por una consulta o tratamiento de un paciente.
/// </summary>
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
