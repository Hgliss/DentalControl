using DentalControl.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DentalControl.Data.Context;

/// <summary>
/// Crea el contexto para ejecutar las migraciones.
/// </summary>
public class AppDbContextFactory
    : IDesignTimeDbContextFactory<AppDbContext>
{
    /// <inheritdoc />
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlite(
            DatabaseProvider.GetConnectionString());

        return new AppDbContext(optionsBuilder.Options);
    }
}