namespace DentalControl.Services.Interface.Authentication;

/// <summary>Permite consultar la identidad de la sesión sin depender de WPF.</summary>
public interface IUserSession
{
    int? UserId { get; }
    bool CanChangePatientStatus { get; }
}
