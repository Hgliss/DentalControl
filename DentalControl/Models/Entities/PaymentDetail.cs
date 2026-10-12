using DentalControl.Models.Common;


namespace DentalControl.Models.Entities;

/// <summary>
/// Representa el importe de un pago aplicado a un cargo.
/// </summary>
public class PaymentDetail : BaseEntity
{
    public int PaymentId { get; set; }

    public int ChargeId { get; set; }

    public decimal AmountApplied { get; set; }

    public Payment Payment { get; set; } = null!;

    public Charge Charge { get; set; } = null!;
}
