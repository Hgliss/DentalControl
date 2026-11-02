using System.ComponentModel.DataAnnotations;
using DentalControl.Models.Entities;

namespace DentalControl.Services.Implemetations.Patients;

/// <summary>
/// Reglas del paciente que no requieren consultar la base de datos.
/// </summary>
public sealed class PatientBusinessRules
{
    public void Validate(Patient patient)
    {
        var errors = GetValidationResults(patient);
        if (errors.Count > 0)
            throw new ValidationException(string.Join(Environment.NewLine, errors.Select(error => error.ErrorMessage)));
    }

    /// <summary>Devuelve las mismas reglas por campo para su presentación en WPF.</summary>
    public static IReadOnlyList<ValidationResult> GetValidationResults(Patient patient)
    {
        ArgumentNullException.ThrowIfNull(patient);
        var errors = new List<ValidationResult>();

        ValidateText(patient.FirstName, nameof(Patient.FirstName), "El primer nombre", 100, required: true);
        ValidateText(patient.MiddleName, nameof(Patient.MiddleName), "El segundo nombre", 100);
        ValidateText(patient.LastName, nameof(Patient.LastName), "El primer apellido", 100, required: true);
        ValidateText(patient.SecondLastName, nameof(Patient.SecondLastName), "El segundo apellido", 100);
        ValidateText(patient.Phone, nameof(Patient.Phone), "El teléfono", 20);
        ValidateText(patient.Address, nameof(Patient.Address), "La dirección", 500);
        ValidateText(patient.Occupation, nameof(Patient.Occupation), "La ocupación", 100);
        ValidateText(patient.ReferredBy, nameof(Patient.ReferredBy), "El campo referido por", 100);

        if (patient.BirthDate == default)
            AddError(nameof(Patient.BirthDate), "La fecha de nacimiento es obligatoria.");
        else if (patient.BirthDate > DateOnly.FromDateTime(DateTime.Today))
            AddError(nameof(Patient.BirthDate), "La fecha de nacimiento no puede ser futura.");

        if (patient.GenderId <= 0)
            AddError(nameof(Patient.GenderId), "Selecciona un género.");

        if (!string.IsNullOrWhiteSpace(patient.Cui))
        {
            var cui = patient.Cui.Trim();
            if (cui.Length != 13 || 
                cui.Any(character => 
                character is < '0' or > '9'))
                AddError(nameof(Patient.Cui), "El CUI debe contener exactamente 13 dígitos de 0 a 9.");
        }

        return errors;

        void AddError(string field, string message) => errors.Add(new ValidationResult(message, [field]));

        void ValidateText(
            string? value, 
            string field,
            string label, 
            int maxLength, 
            bool required = false)
        {
            var text = value?.Trim();
            if (required && string.IsNullOrEmpty(text))
                AddError(field, $"{label} es obligatorio.");
            else if (text?.Length > maxLength)
                AddError(field, $"{label} no puede superar {maxLength} caracteres.");
        }
    }
}
