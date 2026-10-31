using CommunityToolkit.Mvvm.ComponentModel;
using DentalControl.Infrastructure.Navigation;
using DentalControl.Services.Authentication;
using DentalControl.Services.Dialogs;
using DentalControl.ViewModels.Administration;
using DentalControl.ViewModels.Authentication;
using DentalControl.ViewModels.Home;
using DentalControl.ViewModels.Navigation;
using DentalControl.ViewModels.Patients;
using DentalControl.ViewModels.Payments;
using DentalControl.ViewModels.Reports;
using DentalControl.Views;
using Microsoft.Extensions.DependencyInjection;

namespace DentalControl;

/// <summary>Composición de presentación; los contextos y servicios de datos siguen en Data.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services)
    {
        services.AddSingleton<ILoginService, ScopedLoginService>();
        services.AddSingleton<IDialogService, WpfDialogService>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<LoginWindow>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<PatientsViewModel>();
        services.AddTransient<PaymentsViewModel>();
        services.AddTransient<ReportsViewModel>();
        services.AddTransient<AdministrationViewModel>();

        // Las fábricas resuelven instancias nuevas, sin conservar estado de otra pantalla o sesión.
        services.AddSingleton<IScreenFactory>(provider => new ScreenFactory(
            new Dictionary<Screen, Func<ObservableObject>>
            {
                [Screen.Home] = () => provider.GetRequiredService<HomeViewModel>(),
                [Screen.Patients] = () => provider.GetRequiredService<PatientsViewModel>(),
                [Screen.Payments] = () => provider.GetRequiredService<PaymentsViewModel>(),
                [Screen.Reports] = () => provider.GetRequiredService<ReportsViewModel>(),
                [Screen.Administration] = () => provider.GetRequiredService<AdministrationViewModel>()
            }));

        return services;
    }
}
