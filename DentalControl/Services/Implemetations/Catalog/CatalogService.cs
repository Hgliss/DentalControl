using DentalControl.Data.Context;
using DentalControl.Data.Database.Common;
using DentalControl.Models.Catalog;
using DentalControl.Services.Interface.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DentalControl.Services.Implemetations.Catalog;

/// <summary>Consulta y valida catálogos; no inserta ni modifica registros.</summary>
public sealed class CatalogService : ICatalogService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public CatalogService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<IReadOnlyList<CatalogItem>> GetActiveItemsAsync(string categoryCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryCode);

        // El contexto pertenece a esta operación, no a toda la vida de la pantalla.
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await GetActiveItemsQuery(context, categoryCode.Trim())
            .Include(item => item.Category)
            .OrderBy(item => item.OrderIndex ?? int.MaxValue)
            .ThenBy(item => item.Name)
            .ThenBy(item => item.Id)
            .ToListAsync();
    }

    public async Task<bool> IsActiveItemAsync(int itemId, string categoryCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryCode);

        if (itemId <= 0)
        {
            return false;
        }

        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await GetActiveItemsQuery(context, categoryCode.Trim())
            .AnyAsync(item => item.Id == itemId);
    }

    // Una sola definición para que consultar y validar apliquen las mismas reglas.
    private static IQueryable<CatalogItem> GetActiveItemsQuery(
        AppDbContext context, string categoryCode)
    {
        return context.CatalogItems
            .AsNoTracking()
            .Where(item =>
                item.Status == EntityStatus.Active &&
                item.DeletedAt == null &&
                item.Category.Code == categoryCode &&
                item.Category.Status == EntityStatus.Active &&
                item.Category.DeletedAt == null);
    }
}
