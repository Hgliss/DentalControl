
using DentalControl.Models.Entities;

namespace DentalControl.Models.Common;

/// <summary>
/// Entidad base común para todos los módulos del sistema.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }

    public string Status { get; set; } = "A";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedBy { get; set; }

    public User? CreatedByUser { get; set; }

    public User? UpdatedByUser { get; set; }
    public User? DeletedByUser { get; set; }
}