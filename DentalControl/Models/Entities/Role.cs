using DentalControl.Models.Common;

namespace DentalControl.Models.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ICollection<User> Users { get; set; } = new List<User>();
}
