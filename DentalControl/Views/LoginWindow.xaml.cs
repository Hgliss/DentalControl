using System.ComponentModel;
using System.Windows;
using DentalControl.Services.Authentication;
using DentalControl.ViewModels.Authentication;

namespace DentalControl.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public AuthenticatedUser? AuthenticatedUser { get; private set; }

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;

        Loaded += (_, _) => UsernameInput.Focus();
    }

    private async void LoginButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_viewModel.IsBusy)
        {
            return;
        }

        // La validación sin consulta conserva los campos, como antes.
        var hasRequiredInput = _viewModel.HasRequiredInput(PasswordInput.Password);

        try
        {
            AuthenticatedUser = await _viewModel.LoginAsync(PasswordInput.Password);
        }
        finally
        {
            if (hasRequiredInput)
            {
                ClearPasswordControls();
            }
        }

        if (AuthenticatedUser is not null)
        {
            DialogResult = true;
        }
        else if (hasRequiredInput)
        {
            PasswordInput.Focus();
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        // Espera a que termine la consulta antes de cerrar.
        if (_viewModel.IsBusy)
        {
            e.Cancel = true;
        }

        base.OnClosing(e);
    }

    protected override void OnClosed(EventArgs e)
    {
        ClearPasswordControls();
        base.OnClosed(e);
    }

    private void ClearPasswordControls()
    {
        HidePassword();
        PasswordInput.Clear();
    }

    private void HidePassword()
    {
        // Ocultar antes de limpiar evita copiar el vacío al PasswordBox desde TextChanged.
        VisiblePasswordInput.Visibility = Visibility.Collapsed;
        VisiblePasswordInput.Clear();
        PasswordInput.Visibility = Visibility.Visible;
        TogglePasswordButton.ToolTip = "Mostrar contraseña";
        System.Windows.Automation.AutomationProperties.SetName(
            TogglePasswordButton, "Mostrar contraseña");
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

            HidePassword();
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
