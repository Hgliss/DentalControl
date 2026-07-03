using DentalControl.Models.Common;
using DentalControl.Models.Catalog;

namespace DentalControl.Models.Entities;

public class Payment : BaseEntity
{
    public int PatientId { get; set; }

    public int PaymentMethodId { get; set; }

    public decimal AmountPaid { get; set; }

    public DateTime PaymentDate { get; set; }

    public string ReceiptNumber { get; set; } = string.Empty;

    public bool InvoiceRequested { get; set; }

    public Patient Patient { get; set; } = null!;

    public PaymentMethod PaymentMethod { get; set; } = null!;

    public ICollection<PaymentDetail> Details { get; set; } = new List<PaymentDetail>();
}
