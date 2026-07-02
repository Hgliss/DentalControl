
namespace DentalControl.Models.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public ICollection<User> Users { get; set; } = new List<User>();
}
