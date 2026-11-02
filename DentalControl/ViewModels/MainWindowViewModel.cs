using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DentalControl.Services.Implemetations.Authentication;
using DentalControl.Services.Interface.Dialogs;
using DentalControl.ViewModels.Navigation;
using DentalControl.ViewModels.Patients;

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
        LogoutCommand = new RelayCommand(Logout, () => CanLeaveCurrentScreen);
    }

    public string WelcomeText { get; }
    public bool CanAccessAdministration { get; }
    public bool CanLeaveCurrentScreen =>
        CurrentViewModel is not PatientsViewModel patients || (!patients.IsFormOpen && !patients.IsBusy);
    public IRelayCommand<Screen> NavigateCommand { get; }
    public IRelayCommand LogoutCommand { get; }
    public event EventHandler? LogoutRequested;

    public ObservableObject CurrentViewModel
    {
        get => _currentViewModel;
        private set
        {
            if (ReferenceEquals(_currentViewModel, value)) return;
            _currentViewModel.PropertyChanged -= OnScreenPropertyChanged;
            SetProperty(ref _currentViewModel, value);
            _currentViewModel.PropertyChanged += OnScreenPropertyChanged;
            NotifyNavigationState();
        }
    }

    public Screen SelectedScreen
    {
        get => _selectedScreen;
        private set => SetProperty(ref _selectedScreen, value);
    }

    private bool CanNavigate(Screen screen) =>
        CanLeaveCurrentScreen &&
        Enum.IsDefined(screen) &&
        (screen != Screen.Administration || CanAccessAdministration);

    private void Navigate(Screen screen)
    {
        // Comprobar también aquí protege llamadas directas a Execute.
        if (!CanNavigate(screen))
        {
            _dialogs.ShowWarning(CanLeaveCurrentScreen
                ? "No tienes permiso para acceder a Administración."
                : "Guarda o cancela el registro de pacientes antes de cambiar de sección.");
            return;
        }

        CurrentViewModel = _screens.Create(screen);
        SelectedScreen = screen;
    }

    private void Logout()
    {
        if (!CanLeaveCurrentScreen)
        {
            _dialogs.ShowWarning("Guarda o cancela el registro de pacientes antes de cerrar sesión.");
            return;
        }
        LogoutRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnScreenPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(PatientsViewModel.IsFormOpen) or nameof(PatientsViewModel.IsBusy))
            NotifyNavigationState();
    }

    private void NotifyNavigationState()
    {
        OnPropertyChanged(nameof(CanLeaveCurrentScreen));
        NavigateCommand.NotifyCanExecuteChanged();
        LogoutCommand.NotifyCanExecuteChanged();
    }
}
