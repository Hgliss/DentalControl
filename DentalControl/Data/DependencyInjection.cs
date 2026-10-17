using DentalControl.Data.Context;
using DentalControl.Data.Database;
using DentalControl.Data.initialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DentalControl.Models.Entities;
using Microsoft.AspNetCore.Identity;


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

        services.AddScoped<DatabaseInitializer>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        return services;
    }
}