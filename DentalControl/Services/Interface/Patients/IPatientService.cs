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

    /// <summary>
    /// Consulta pacientes sin seguimiento, ordenados por el número de su código.
    /// Busca por nombres, apellidos, CUI y código, sin distinguir mayúsculas ni tildes.
    /// Todos incluye activos e inactivos; los eliminados lógicamente siempre se excluyen.
    /// </summary>
    /// <remarks>
    /// El tamaño predeterminado es 10 y se permiten de 1 a 100 registros por página.
    /// Si la página excede el total, se devuelve la última disponible. Sin resultados,
    /// Items está vacío, TotalCount es 0 y PageNumber es 1. Los errores se propagan.
    /// </remarks>
    Task<(IReadOnlyList<Patient> Items, int TotalCount, int PageNumber)> GetPageAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchText = null,
        PatientStatusFilter status = PatientStatusFilter.Active,
        CancellationToken cancellationToken = default);
}
