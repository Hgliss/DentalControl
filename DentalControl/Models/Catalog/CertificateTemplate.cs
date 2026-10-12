using DentalControl.Models.Entities;

namespace DentalControl.Models.Catalog;

/// <summary>
/// Representa una plantilla para generar constancias médicas.
/// </summary>
public class CertificateTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<MedicalCertificate> Certificates { get; set; }
        = new List<MedicalCertificate>();
}