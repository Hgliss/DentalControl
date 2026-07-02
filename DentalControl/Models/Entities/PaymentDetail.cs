using DentalControl.Models.Common;
using DentalControl.Models.Catalog;


namespace DentalControl.Models.Entities;

public class PaymentDetail
{
    public int Id { get; set; }

    public int PaymentId { get; set; }

    public int ChargeId { get; set; }

    public decimal AmountApplied { get; set; }

    public Payment Payment { get; set; } = null!;

    public Charge Charge { get; set; } = null!;
}
