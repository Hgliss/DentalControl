using System;
using System.Windows;
using DentalControl.Data;
using DentalControl.Data.Context;
using DentalControl.Data.initialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DentalControl;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Mantiene la aplicación abierta durante la inicialización.
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        try
        {
            var services = new ServiceCollection();

            services.AddDataServices();

            _serviceProvider = services.BuildServiceProvider(
                new ServiceProviderOptions
                {
                    ValidateScopes = true
                });

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

                await context.Database.MigrateAsync();

                var initializer = scope.ServiceProvider
                    .GetRequiredService<DatabaseInitializer>();

                await initializer.InitializeAsync();
            }

            var window = new MainWindow();

            MainWindow = window;
            ShutdownMode = ShutdownMode.OnMainWindowClose;

            window.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo inicializar la aplicación.\n\n{ex.Message}",
                "DentalControl",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown(1);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();

        base.OnExit(e);
    }
}