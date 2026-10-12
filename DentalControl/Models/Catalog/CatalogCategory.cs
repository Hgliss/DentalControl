
using DentalControl.Models.Common;

namespace DentalControl.Models.Catalog;

public class CatalogCategory : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public virtual ICollection<CatalogItem> Items { get; set; } = new List<CatalogItem>();
}
