namespace DentalControl.Services.Interface.Dialogs;

public interface IDialogService
{
    void ShowWarning(string message);
    bool Confirm(string message);
}
