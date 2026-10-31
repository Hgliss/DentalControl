using System.Windows;
using DentalControl.ViewModels;

namespace DentalControl;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}
