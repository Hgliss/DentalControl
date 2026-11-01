namespace DentalControl.DTOs.Patients;

/// <summary>
/// Datos de consulta de un paciente, incluidos su identificador y código generado.
/// Es independiente de las solicitudes de escritura y no incluye el expediente clínico.
/// </summary>
public class PatientResponseDto
{
    public int Id { get; set; }

    public string PatientCode { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = string.Empty;

    public string? SecondLastName { get; set; }

    public DateOnly BirthDate { get; set; }

    public int GenderId { get; set; }

    public string GenderName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Cui { get; set; }

    public string? Occupation { get; set; }

    public string? ReferredBy { get; set; }

    public string Status { get; set; } = string.Empty;
}
