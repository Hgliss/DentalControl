using DentalControl.Data.Context;
using DentalControl.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace DentalControl.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDataServices(
        this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(DatabaseProvider.GetConnectionString());
        });

        return services;
    }
}