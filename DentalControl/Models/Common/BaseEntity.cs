
using DentalControl.Models.Entities;

namespace DentalControl.Models.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public User? CreatedByUser { get; set; }

    public User? UpdatedByUser { get; set; }
}