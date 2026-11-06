using System.Windows;
using DentalControl.Services.Interface.Dialogs;

namespace DentalControl.Services.Implemetations.Dialogs;

public sealed class WpfDialogService : IDialogService
{
    public bool Confirm(string message) =>
        MessageBox.Show(message, "DentalControl", MessageBoxButton.YesNo,
            MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes;

    public void ShowWarning(string message)
    {
        MessageBox.Show(message, "DentalControl", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
