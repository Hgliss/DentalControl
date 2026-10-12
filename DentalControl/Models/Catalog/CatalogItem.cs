using DentalControl.Models.Common;

namespace DentalControl.Models.Catalog;

public class CatalogItem : BaseEntity
{
    public int CategoryId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? OrderIndex { get; set; }

    public CatalogCategory Category { get; set; } = null!;
}
