using System.ComponentModel.DataAnnotations;

namespace DentalControl.DTOs.Patients;

/// <summary>
/// Campos editables compartidos por el registro y la actualización completa de pacientes.
/// No incluye identificador, código automático, estado ni auditoría.
/// </summary>
public abstract class PatientWriteDto
{
    [Required(ErrorMessage = "El primer nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El primer nombre no puede superar 100 caracteres.")]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "El segundo nombre no puede superar 100 caracteres.")]
    public string? MiddleName { get; set; }

    [Required(ErrorMessage = "El primer apellido es obligatorio.")]
    [StringLength(100, ErrorMessage = "El primer apellido no puede superar 100 caracteres.")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "El segundo apellido no puede superar 100 caracteres.")]
    public string? SecondLastName { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    public DateOnly? BirthDate { get; set; }

    [Required(ErrorMessage = "Debes seleccionar un género.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un género válido.")]
    public int? GenderId { get; set; }

    [StringLength(20, ErrorMessage = "El teléfono no puede superar 20 caracteres.")]
    public string? Phone { get; set; }

    [StringLength(500, ErrorMessage = "La dirección no puede superar 500 caracteres.")]
    public string? Address { get; set; }

    [RegularExpression(@"^[0-9]{13}$", ErrorMessage = "El CUI debe contener exactamente 13 dígitos.")]
    public string? Cui { get; set; }

    [StringLength(100, ErrorMessage = "La ocupación no puede superar 100 caracteres.")]
    public string? Occupation { get; set; }

    [StringLength(100, ErrorMessage = "El referido por no puede superar 100 caracteres.")]
    public string? ReferredBy { get; set; }
}
