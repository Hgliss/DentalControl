using System.Windows;

namespace DentalControl;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var app = (App)Application.Current;
        var user = app.CurrentUser;

        WelcomeText.Text = user is null
            ? ""
            : $"Sesión: {user.FullName} · {user.RoleName}";
    }

    private void LogoutButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var app = (App)Application.Current;

        app.Logout();
    }
}