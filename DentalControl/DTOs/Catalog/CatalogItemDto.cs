namespace DentalControl.DTOs.Catalog;

/// <summary>
/// Datos de una opción de catálogo para su uso en formularios.
/// </summary>
public class CatalogItemDto
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
}