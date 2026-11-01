using DentalControl.DTOs.Catalog;


namespace DentalControl.Services.Interface.Catalog;

public interface ICatalogService
{
    /// <summary>
    /// Obtiene los ítems activos de una categoría activa.
    /// </summary>
    Task<IReadOnlyList<CatalogItemDto>> GetActiveItemAsync(
        string categoryCode);

    /// <summary>
    /// Comprueba que el ítem esté activvo y pertenzca
    /// a la categoria activa indicada.     
    /// </summary>
    Task<bool> IsActiveItemAsync(
        int itemId,
        string categoryCode);
}
