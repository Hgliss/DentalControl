using CommunityToolkit.Mvvm.ComponentModel;
using DentalControl.ViewModels.Navigation;

namespace DentalControl.Infrastructure.Navigation;

public sealed class ScreenFactory(
    IReadOnlyDictionary<Screen, Func<ObservableObject>> screens) : IScreenFactory
{
    public ObservableObject Create(Screen screen)
    {
        if (!screens.TryGetValue(screen, out var create))
        {
            throw new ArgumentOutOfRangeException(nameof(screen), screen, "Pantalla no registrada.");
        }

        return create();
    }
}
