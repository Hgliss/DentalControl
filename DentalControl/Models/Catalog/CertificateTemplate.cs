using DentalControl.Models.Catalog;
using DentalControl.Models.Common;
using DentalControl.Models.Entities;

namespace DentalControl.Models.Catalog;

public class CertificateTemplate 
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<MedicalCertificate> Certificates { get; set; } = new List<MedicalCertificate>();
}
