using System.ComponentModel;
using System.Windows;
using DentalControl.Services.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace DentalControl.Views;

public partial class LoginWindow : Window
{
    private readonly IServiceScopeFactory _scopeFactory;
    private bool _isBusy;

    public AuthenticatedUser? AuthenticatedUser { get; private set; }

    public LoginWindow(IServiceScopeFactory scopeFactory)
    {
        InitializeComponent();

        _scopeFactory = scopeFactory;

        Loaded += (_, _) => UsernameInput.Focus();
    }

    private async void LoginButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_isBusy)
        {
            return;
        }

        ErrorText.Text = "";

        if (string.IsNullOrWhiteSpace(UsernameInput.Text) ||
            string.IsNullOrEmpty(PasswordInput.Password))
        {
            ErrorText.Text = "Ingresa tu usuario y contraseña.";
            return;
        }

        _isBusy = true;
        LoginButton.IsEnabled = false;
        UsernameInput.IsEnabled = false;
        PasswordInput.IsEnabled = false;
        VisiblePasswordInput.IsEnabled = false;
        TogglePasswordButton.IsEnabled = false;
        LoginButton.Content = "Ingresando…";

        try
        {
            // Cada intento utiliza un contexto nuevo.
            using var scope = _scopeFactory.CreateScope();

            var authenticationService = scope.ServiceProvider
                .GetRequiredService<AuthenticationService>();

            AuthenticatedUser = await authenticationService.LoginAsync(
                UsernameInput.Text,
                PasswordInput.Password);

            if (AuthenticatedUser is null)
            {
                ErrorText.Text =
                    "No se pudo iniciar sesión. Verifica tus credenciales " +
                    "o el estado de tu cuenta.";
            }
        }
        catch (Exception)
        {
            ErrorText.Text =
                "Ocurrió un error al consultar el acceso. Intenta nuevamente.";
        }
        finally
        {
            PasswordInput.Clear();
            VisiblePasswordInput.Clear();

            PasswordInput.Visibility = Visibility.Visible;
            VisiblePasswordInput.Visibility = Visibility.Collapsed;
            TogglePasswordButton.ToolTip = "Mostrar contraseña";
            System.Windows.Automation.AutomationProperties.SetName(
                TogglePasswordButton, "Mostrar contraseña");

            _isBusy = false;
            LoginButton.IsEnabled = true;
            UsernameInput.IsEnabled = true;
            PasswordInput.IsEnabled = true;
            VisiblePasswordInput.IsEnabled = true;
            TogglePasswordButton.IsEnabled = true;
            LoginButton.Content = "Ingresar";
        }

        if (AuthenticatedUser is not null)
        {
            DialogResult = true;
        }
        else
        {
            PasswordInput.Focus();
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        // Espera a que termine la consulta antes de cerrar.
        if (_isBusy)
        {
            e.Cancel = true;
        }

        base.OnClosing(e);
    }

    private void TogglePasswordButton_Click(
    object sender, RoutedEventArgs e)
    {
        if (PasswordInput.Visibility == Visibility.Visible)
        {
            VisiblePasswordInput.Text = PasswordInput.Password;

            PasswordInput.Visibility = Visibility.Collapsed;
            VisiblePasswordInput.Visibility = Visibility.Visible;

            TogglePasswordButton.ToolTip = "Ocultar contraseña";
            System.Windows.Automation.AutomationProperties.SetName(
                TogglePasswordButton, "Ocultar contraseña");

            VisiblePasswordInput.Focus();
            VisiblePasswordInput.CaretIndex =
                VisiblePasswordInput.Text.Length;
        }
        else
        {
            PasswordInput.Password = VisiblePasswordInput.Text;

            VisiblePasswordInput.Visibility = Visibility.Collapsed;
            PasswordInput.Visibility = Visibility.Visible;

            TogglePasswordButton.ToolTip = "Mostrar contraseña";
            System.Windows.Automation.AutomationProperties.SetName(
                TogglePasswordButton, "Mostrar contraseña");

            PasswordInput.Focus();
        }
    }

    private void VisiblePasswordInput_TextChanged(
        object sender,
        System.Windows.Controls.TextChangedEventArgs e)
    {
        if (VisiblePasswordInput.Visibility == Visibility.Visible)
        {
            PasswordInput.Password = VisiblePasswordInput.Text;
        }
    }
}
