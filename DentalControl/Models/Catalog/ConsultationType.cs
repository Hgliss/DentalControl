using DentalControl.Models.Entities;

namespace DentalControl.Models.Catalog;
public class ConsultationType 
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();

}
