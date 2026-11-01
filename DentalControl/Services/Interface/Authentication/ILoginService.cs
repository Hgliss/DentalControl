using DentalControl.Services.Implemetations.Authentication;

namespace DentalControl.Services.Interface.Authentication;

/// <summary>Acceso a la autenticación sin exponer EF Core ni sus alcances a la presentación.</summary>
public interface ILoginService
{
    Task<AuthenticatedUser?> LoginAsync(string username, string password);
}
