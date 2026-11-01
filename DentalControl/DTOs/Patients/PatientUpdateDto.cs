namespace DentalControl.DTOs.Patients;

/// <summary>
/// Datos para actualizar todos los campos editables de un paciente.
/// El identificador se recibe por separado en la operación del servicio.
/// Los campos opcionales vacíos podrán limpiarse al normalizar y guardar.
/// </summary>
public class PatientUpdateDto : PatientWriteDto
{
}
