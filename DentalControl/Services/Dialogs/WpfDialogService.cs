using System.Windows;

namespace DentalControl.Services.Dialogs;

public sealed class WpfDialogService : IDialogService
{
    public void ShowWarning(string message)
    {
        MessageBox.Show(message, "DentalControl", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
