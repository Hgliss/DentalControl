using DentalControl.Data.Context;
using DentalControl.Data.Database.Common;
using DentalControl.DTOs.Catalog;
using DentalControl.Models.Catalog;
using DentalControl.Services.Interface.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DentalControl.Services.Implemetations.Catalog;

public class CatalogService : ICatalogService
{
    public readonly IServiceScopeFactory _scopeFactory;

    public CatalogService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<IReadOnlyList<CatalogItemDto>> GetActiveItemAsync(
        string categoryCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryCode);

        await using var scope = _scopeFactory.CreateAsyncScope();

        var context = scope.ServiceProvider
             .GetRequiredService<AppDbContext>();

        return await GetActiveItemsQuery(context, categoryCode.Trim())
            .OrderBy(item => item.OrderIndex ?? int.MaxValue)
            .ThenBy(item => item.Name)
            .ThenBy(item => item.Id)
            .Select(item => new CatalogItemDto
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name
            })
            .ToListAsync();
        }

    public async Task<bool> IsActiveItemAsync(
        int itemId,
        string categoryCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryCode);

        if (itemId <= 0)
        {
            return false;
        }

        await using var scope = _scopeFactory.CreateAsyncScope();

        var context = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        return await GetActiveItemsQuery(context, categoryCode.Trim())
            .AnyAsync(item => item.Id == itemId);
    }


    private static IQueryable<CatalogItem> GetActiveItemsQuery(
        AppDbContext context,
        string categoryCode)
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
