using DentalControl.Models.Entities;

namespace DentalControl.Models.Catalog;
public class TreatmentType
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
}


