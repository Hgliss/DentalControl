using DentalControl.Models.Common;
using DentalControl.Models.Catalog;

namespace DentalControl.Models.Entities;

public class MedicalCertificate : AuditableEntity
{
    public int Id { get; set; }

    public int ConsultationId { get; set; }

    public int? TemplateId { get; set; }

    public DateTime IssueDate { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public Consultation Consultation { get; set; } = null!;

    public CertificateTemplate? Template { get; set; }
}
