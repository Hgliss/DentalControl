using CommunityToolkit.Mvvm.ComponentModel;

namespace DentalControl.ViewModels.Navigation;

public interface IScreenFactory
{
    ObservableObject Create(Screen screen);
}
