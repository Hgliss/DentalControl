using System.DirectoryServices.ActiveDirectory;

namespace DentalControl.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Email { get; set; }
    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
