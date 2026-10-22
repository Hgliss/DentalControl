using System.Windows;
using System.Windows.Media;

namespace DentalControl;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var app = (App)Application.Current;
        var user = app.CurrentUser;

        if (user is null)
        {
            throw new System.InvalidOperationException(
                "No se puede abrir la ventana principal sin una sesión activa.");
        }

        WelcomeText.Text =
            $"Sesión: {user.FullName} · {user.RoleName}";

        AdministrationButton.Visibility =
            user.RoleName == "Administrador"
                ? Visibility.Visible
                : Visibility.Collapsed;

        MainContent.Content = new DentalControl.Views.HomeView();
    }

    private void HomeButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        MainContent.Content = new DentalControl.Views.HomeView();

        ClearMenuSelection();

        HomeButton.Background =
            new SolidColorBrush(Color.FromRgb(234, 240, 251));
    }

    private void PatientsButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        MainContent.Content = new DentalControl.Views.PatientsView();

        ClearMenuSelection();

        PatientsButton.Background =
            new SolidColorBrush(Color.FromRgb(234, 240, 251));
    }

    private void PaymentsButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        MainContent.Content = new DentalControl.Views.PaymentsView();

        ClearMenuSelection();

        PaymentsButton.Background =
            new SolidColorBrush(Color.FromRgb(234, 240, 251));
    }

    private void ReportsButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        MainContent.Content = new DentalControl.Views.ReportsView();

        ClearMenuSelection();

        ReportsButton.Background =
            new SolidColorBrush(Color.FromRgb(234, 240, 251));
    }

    private void AdministrationButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var app = (App)Application.Current;

        if (app.CurrentUser?.RoleName != "Administrador")
        {
            MessageBox.Show(
                "No tienes permiso para acceder a Administración.",
                "DentalControl",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        MainContent.Content =
            new DentalControl.Views.AdministrationView();

        ClearMenuSelection();

        AdministrationButton.Background =
            new SolidColorBrush(Color.FromRgb(234, 240, 251));
    }


    private void LogoutButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var app = (App)Application.Current;

        app.Logout();
    }

    private void ClearMenuSelection()
    {
        HomeButton.Background = Brushes.Transparent;
        PatientsButton.Background = Brushes.Transparent;
        PaymentsButton.Background = Brushes.Transparent;
        ReportsButton.Background = Brushes.Transparent;
        AdministrationButton.Background = Brushes.Transparent;
    }
}