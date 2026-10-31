using CommunityToolkit.Mvvm.ComponentModel;
using DentalControl;
using DentalControl.Infrastructure.Converters;
using DentalControl.Services.Authentication;
using DentalControl.Services.Dialogs;
using DentalControl.ViewModels;
using DentalControl.ViewModels.Administration;
using DentalControl.ViewModels.Authentication;
using DentalControl.ViewModels.Home;
using DentalControl.ViewModels.Navigation;
using DentalControl.ViewModels.Patients;
using DentalControl.ViewModels.Payments;
using DentalControl.ViewModels.Reports;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows;

// Ejecutable independiente: no crea App, ventanas, DbContext ni conexiones SQLite.
var checks = 0;
void Check(bool condition, string description)
{
    if (!condition)
    {
        throw new InvalidOperationException("FALLÓ: " + description);
    }

    checks++;
    Console.WriteLine("OK: " + description);
}

var administrator = new AuthenticatedUser(1, "admin", "Administrador de prueba", "Administrador");
var doctor = new AuthenticatedUser(2, "doctor", "Doctor de prueba", "Doctor");
var login = new FakeLoginService();
var vm = new LoginViewModel(login);
var notifications = new List<string?>();
vm.PropertyChanged += (_, e) => notifications.Add(e.PropertyName);

Check(vm.CanEdit && !vm.IsBusy && vm.LoginButtonText == "Ingresar", "Estado inicial del login");
Check(await vm.LoginAsync("prueba") is null && login.Calls == 0, "Usuario vacío no consulta el servicio");
vm.Username = "admin";
Check(await vm.LoginAsync("") is null && login.Calls == 0, "Contraseña vacía no consulta el servicio");
Check(vm.ErrorMessage == "Ingresa tu usuario y contraseña.", "Mensaje de campos obligatorios");

var completion = new TaskCompletionSource<AuthenticatedUser?>(TaskCreationOptions.RunContinuationsAsynchronously);
login.Handler = (_, _) => completion.Task;
var pending = vm.LoginAsync("prueba");
Check(vm.IsBusy && !vm.CanEdit && vm.LoginButtonText == "Ingresando…", "Consulta pendiente bloquea la edición");
Check(vm.ErrorMessage == "", "Un intento nuevo limpia el error anterior");
Check(await vm.LoginAsync("otro intento") is null && login.Calls == 1, "No se superponen intentos");
completion.SetResult(null);
Check(await pending is null && !vm.IsBusy && vm.CanEdit, "Rechazo restablece la edición");
Check(vm.Username == "admin" && vm.ErrorMessage.Contains("Verifica tus credenciales"), "Rechazo conserva usuario y mensaje general");
Check(new[] { "Username", "ErrorMessage", "IsBusy", "CanEdit", "LoginButtonText" }
    .All(name => notifications.Contains(name)), "Notificaciones de propiedades enlazadas");

login.Handler = (_, _) => throw new InvalidOperationException("Detalle privado de prueba");
Check(await vm.LoginAsync("prueba") is null && !vm.IsBusy && vm.CanEdit, "Excepción restablece el estado");
Check(vm.ErrorMessage == "Ocurrió un error al consultar el acceso. Intenta nuevamente.", "Excepción no expone detalles internos");

var credentialsForwarded = false;
login.Handler = (username, password) =>
{
    credentialsForwarded = username == "admin" && password == "prueba";
    return Task.FromResult<AuthenticatedUser?>(administrator);
};
Check(await vm.LoginAsync("prueba") == administrator && credentialsForwarded, "Acceso correcto después de un error");
Check(vm.ErrorMessage == "" && vm.CanEdit, "Éxito limpia el error y termina el estado ocupado");
Check(!typeof(LoginViewModel).GetProperties().Any(p => p.Name.Contains("Password", StringComparison.OrdinalIgnoreCase)),
    "La contraseña no es una propiedad del ViewModel");

using var services = new ServiceCollection().AddPresentationServices()
    .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
Check(!ReferenceEquals(services.GetRequiredService<LoginViewModel>(), services.GetRequiredService<LoginViewModel>()),
    "Cada login obtiene un ViewModel nuevo");
var screens = services.GetRequiredService<IScreenFactory>();
var dialogs = new FakeDialogService();
var main = ActivatorUtilities.CreateInstance<MainWindowViewModel>(services, administrator, dialogs);
Check(main.CurrentViewModel is HomeViewModel && main.SelectedScreen == Screen.Home, "Inicio es la pantalla inicial");
Check(main.WelcomeText.Contains(administrator.FullName) && main.CanAccessAdministration, "Sesión de administrador");

var screenTypes = new Dictionary<Screen, Type>
{
    [Screen.Home] = typeof(HomeViewModel),
    [Screen.Patients] = typeof(PatientsViewModel),
    [Screen.Payments] = typeof(PaymentsViewModel),
    [Screen.Reports] = typeof(ReportsViewModel),
    [Screen.Administration] = typeof(AdministrationViewModel)
};
foreach (var (screen, expectedType) in screenTypes)
{
    Check(main.NavigateCommand.CanExecute(screen), "Administrador puede navegar a " + screen);
    main.NavigateCommand.Execute(screen);
    Check(main.SelectedScreen == screen && main.CurrentViewModel.GetType() == expectedType,
        "Destino y selección coinciden: " + screen);
    Check(!ReferenceEquals(screens.Create(screen), screens.Create(screen)), "Instancias independientes: " + screen);
}

main.NavigateCommand.Execute(Screen.Patients);
var firstVisit = main.CurrentViewModel;
main.NavigateCommand.Execute(Screen.Patients);
Check(!ReferenceEquals(firstVisit, main.CurrentViewModel), "Volver a una pantalla no reutiliza su estado");

var logoutRequested = false;
main.LogoutRequested += (_, _) => logoutRequested = true;
main.LogoutCommand.Execute(null);
Check(logoutRequested, "Cerrar sesión solicita la transición sin manipular ventanas");

var doctorMain = new MainWindowViewModel(doctor, screens, dialogs);
Check(!doctorMain.CanAccessAdministration && !doctorMain.NavigateCommand.CanExecute(Screen.Administration),
    "Doctor no tiene disponible Administración");
var initialPage = doctorMain.CurrentViewModel;
doctorMain.NavigateCommand.Execute(Screen.Administration);
Check(ReferenceEquals(initialPage, doctorMain.CurrentViewModel) && doctorMain.SelectedScreen == Screen.Home,
    "Execute directo tampoco permite Administración");
Check(dialogs.Warnings == 1, "Acceso directo rechazado avisa mediante el servicio");
Check(doctorMain.WelcomeText.Contains(doctor.FullName), "Una sesión nueva muestra su propio usuario");
foreach (var screen in Enum.GetValues<Screen>().Where(screen => screen != Screen.Administration))
{
    Check(doctorMain.NavigateCommand.CanExecute(screen), "Doctor puede navegar a " + screen);
}

var converter = new ValuesEqualConverter();
Check((bool)converter.Convert([Screen.Home, Screen.Home], typeof(bool), null!, CultureInfo.InvariantCulture),
    "Resaltado cuando coincide la opción");
Check(!(bool)converter.Convert([Screen.Home, Screen.Patients], typeof(bool), null!, CultureInfo.InvariantCulture),
    "Sin resaltado para las otras opciones");
Check(!(bool)converter.Convert([DependencyProperty.UnsetValue, Screen.Home], typeof(bool), null!, CultureInfo.InvariantCulture),
    "Binding pendiente no produce una selección falsa");

Console.WriteLine($"Verificación completada: {checks} comprobaciones. Sin abrir DentalControl ni acceder a SQLite.");

sealed class FakeLoginService : ILoginService
{
    public int Calls { get; private set; }
    public Func<string, string, Task<AuthenticatedUser?>> Handler { get; set; } =
        (_, _) => Task.FromResult<AuthenticatedUser?>(null);

    public Task<AuthenticatedUser?> LoginAsync(string username, string password)
    {
        Calls++;
        return Handler(username, password);
    }
}

sealed class FakeDialogService : IDialogService
{
    public int Warnings { get; private set; }
    public void ShowWarning(string message) => Warnings++;
}
