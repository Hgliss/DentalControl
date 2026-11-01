using DentalControl.Models.Catalog;

namespace DentalControl.Services.Interface.Catalog;

public interface ICatalogService
{
    /// <summary>
    /// Devuelve entidades sin seguimiento de los ítems activos de una categoría activa.
    /// Si la categoría no existe o está inactiva, devuelve una lista vacía.
    /// </summary>
    Task<IReadOnlyList<CatalogItem>> GetActiveItemsAsync(string categoryCode);

    /// <summary>
    /// Comprueba que el ítem exista, esté activo y pertenezca a la categoría activa indicada.
    /// </summary>
    Task<bool> IsActiveItemAsync(int itemId, string categoryCode);
}
