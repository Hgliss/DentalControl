using System.ComponentModel.DataAnnotations;
using DentalControl.DTOs.Patients;

namespace DentalControl.Services.Interface.Patients;

public interface IPatientService
{
    /// <summary>Normaliza los campos del DTO y ejecuta sus validaciones y reglas de negocio.</summary>
    Task<IReadOnlyList<ValidationResult>> ValidateCreateAsync(PatientCreateDto request);

    /// <summary>
    /// Normaliza y valida la actualización completa. El identificador se recibe por separado.
    /// </summary>
    Task<IReadOnlyList<ValidationResult>> ValidateUpdateAsync(int patientId, PatientUpdateDto request);
}
