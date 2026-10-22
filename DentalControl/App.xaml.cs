using System;
using System.Windows;
using DentalControl.Data;
using DentalControl.Data.Context;
using DentalControl.Data.initialization;
using DentalControl.Services.Authentication;
using DentalControl.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DentalControl;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    public AuthenticatedUser? CurrentUser { get; private set; }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

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

            ShowLogin();
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

    private void ShowLogin()
    {
        if (_serviceProvider is null)
        {
            throw new InvalidOperationException(
                "Los servicios no están inicializados.");
        }

        var scopeFactory = _serviceProvider
            .GetRequiredService<IServiceScopeFactory>();

        var loginWindow = new LoginWindow(scopeFactory);

        MainWindow = loginWindow;

        if (loginWindow.ShowDialog() != true ||
            loginWindow.AuthenticatedUser is null)
        {
            Shutdown();
            return;
        }

        CurrentUser = loginWindow.AuthenticatedUser;

        var window = new MainWindow();

        MainWindow = window;
        ShutdownMode = ShutdownMode.OnMainWindowClose;

        window.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        CurrentUser = null;
        _serviceProvider?.Dispose();

        base.OnExit(e);
    }

    public void Logout()
    {
        // Evita cerrar la aplicación mientras cambiamos de ventana.
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        CurrentUser = null;

        var currentWindow = MainWindow;

        MainWindow = null;

        currentWindow?.Close();

        ShowLogin();
    }
}