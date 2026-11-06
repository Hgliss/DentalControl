using DentalControl.Services.Interface.Authentication;

namespace DentalControl.Services.Implemetations.Authentication;

/// <summary>Conserva el usuario autenticado hasta cerrar sesión o salir de la aplicación.</summary>
public sealed class UserSession : IUserSession
{
    public AuthenticatedUser? User { get; private set; }
    public int? UserId => User?.Id;
    public bool CanChangePatientStatus => User is { Id: > 0, RoleName: "Administrador" or "Doctor" };

    public void SetUser(AuthenticatedUser user)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(user.Id);
        User = user;
    }

    public void Clear() => User = null;
}
