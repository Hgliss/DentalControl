using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DentalControl.Services.Authentication;
using DentalControl.Services.Dialogs;
using DentalControl.ViewModels.Navigation;

namespace DentalControl.ViewModels;

public sealed class MainWindowViewModel : ObservableObject
{
    private readonly IScreenFactory _screens;
    private readonly IDialogService _dialogs;
    private ObservableObject _currentViewModel;
    private Screen _selectedScreen = Screen.Home;

    public MainWindowViewModel(
        AuthenticatedUser user, IScreenFactory screens, IDialogService dialogs)
    {
        ArgumentNullException.ThrowIfNull(user);
        _screens = screens;
        _dialogs = dialogs;
        WelcomeText = $"Sesión: {user.FullName} · {user.RoleName}";
        CanAccessAdministration = user.RoleName == "Administrador";
        _currentViewModel = screens.Create(Screen.Home);
        NavigateCommand = new RelayCommand<Screen>(Navigate, CanNavigate);
        LogoutCommand = new RelayCommand(() => LogoutRequested?.Invoke(this, EventArgs.Empty));
    }

    public string WelcomeText { get; }
    public bool CanAccessAdministration { get; }
    public IRelayCommand<Screen> NavigateCommand { get; }
    public IRelayCommand LogoutCommand { get; }
    public event EventHandler? LogoutRequested;

    public ObservableObject CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

    public Screen SelectedScreen
    {
        get => _selectedScreen;
        private set => SetProperty(ref _selectedScreen, value);
    }

    private bool CanNavigate(Screen screen) =>
        Enum.IsDefined(screen) &&
        (screen != Screen.Administration || CanAccessAdministration);

    private void Navigate(Screen screen)
    {
        // Comprobar también aquí protege llamadas directas a Execute.
        if (!CanNavigate(screen))
        {
            _dialogs.ShowWarning("No tienes permiso para acceder a Administración.");
            return;
        }

        CurrentViewModel = _screens.Create(screen);
        SelectedScreen = screen;
    }
}
