using CommunityToolkit.Mvvm.ComponentModel;
using DentalControl.Services.Authentication;

namespace DentalControl.ViewModels.Authentication;

public sealed class LoginViewModel(ILoginService loginService) : ObservableObject
{
    private string _username = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (SetProperty(ref _isBusy, value))
            {
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(LoginButtonText));
            }
        }
    }

    public bool CanEdit => !IsBusy;
    public string LoginButtonText => IsBusy ? "Ingresando…" : "Ingresar";

    public bool HasRequiredInput(string password) =>
        !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrEmpty(password);

    // La contraseña es un argumento temporal; no forma parte del estado enlazado.
    public async Task<AuthenticatedUser?> LoginAsync(string password)
    {
        if (IsBusy)
        {
            return null;
        }

        ErrorMessage = string.Empty;
        if (!HasRequiredInput(password))
        {
            ErrorMessage = "Ingresa tu usuario y contraseña.";
            return null;
        }

        IsBusy = true;
        try
        {
            var user = await loginService.LoginAsync(Username, password);
            if (user is null)
            {
                ErrorMessage = "No se pudo iniciar sesión. Verifica tus credenciales " +
                    "o el estado de tu cuenta.";
            }

            return user;
        }
        catch (Exception)
        {
            ErrorMessage = "Ocurrió un error al consultar el acceso. Intenta nuevamente.";
            return null;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
