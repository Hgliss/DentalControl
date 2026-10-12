using DentalControl.Models.Catalog;
using DentalControl.Models.Common;

namespace DentalControl.Models.Entities;

/// <summary>
/// Representa una constancia médica emitida para una consulta.
/// </summary>
public class MedicalCertificate : BaseEntity
{
    public int ConsultationId { get; set; }
    public int? TemplateId { get; set; }
    public DateTime IssueDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public Consultation Consultation { get; set; } = null!;
    public CertificateTemplate? Template { get; set; }
}