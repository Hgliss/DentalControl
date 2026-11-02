using DentalControl.Models.Entities;

namespace DentalControl.Services.Interface.Patients;

public interface IPatientService
{
    /// <summary>
    /// Registra un paciente y devuelve la entidad guardada con su identificador y código automático.
    /// </summary>
    Task<Patient> CreateAsync(
        Patient patient,
        CancellationToken cancellationToken = default);
}
