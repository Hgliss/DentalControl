using System.ComponentModel.DataAnnotations;
using DentalControl.DTOs.Patients;

namespace DentalControl.Services.Interface.Patients;

public interface IPatientBusinessRules
{
    /// <summary>
    /// Comprueba reglas de negocio sobre datos ya normalizados y con anotaciones validadas.
    /// patientId es null al crear; al actualizar identifica al paciente y lo excluye
    /// de la búsqueda de CUI duplicado.
    /// </summary>
    Task<IReadOnlyList<ValidationResult>> ValidateAsync(
        PatientWriteDto request, int? patientId = null);
}
