using System.Windows;
using System.ComponentModel;
using DentalControl.ViewModels;

namespace DentalControl;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (DataContext is MainWindowViewModel { CanLeaveCurrentScreen: false })
        {
            e.Cancel = true;
            MessageBox.Show(this,
                "Guarda o cancela el registro de pacientes antes de cerrar la ventana.",
                "DentalControl", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        base.OnClosing(e);
    }
}
