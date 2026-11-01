using System.ComponentModel.DataAnnotations;
using DentalControl.DTOs.Patients;
using DentalControl.Services.Interface.Patients;
using Microsoft.Extensions.DependencyInjection;

namespace DentalControl.Services.Implemetations.Patients;

/// <summary>Prepara datos y coordina su validación; no registra ni actualiza pacientes.</summary>
public sealed class PatientService(IServiceScopeFactory scopeFactory) : IPatientService
{
    public Task<IReadOnlyList<ValidationResult>> ValidateCreateAsync(PatientCreateDto request) =>
        ValidateAsync(request, null);

    public Task<IReadOnlyList<ValidationResult>> ValidateUpdateAsync(
        int patientId, PatientUpdateDto request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (patientId <= 0)
        {
            IReadOnlyList<ValidationResult> errors =
                [new ValidationResult("El identificador del paciente debe ser mayor que cero.")];
            return Task.FromResult(errors);
        }

        return ValidateAsync(request, patientId);
    }

    private async Task<IReadOnlyList<ValidationResult>> ValidateAsync(
        PatientWriteDto request, int? patientId)
    {
        ArgumentNullException.ThrowIfNull(request);
        Normalize(request);

        var errors = new List<ValidationResult>();
        if (!Validator.TryValidateObject(request, new ValidationContext(request), errors,
                validateAllProperties: true))
        {
            // No consultar la base hasta que el formato y los campos obligatorios sean válidos.
            return errors;
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        var rules = scope.ServiceProvider.GetRequiredService<IPatientBusinessRules>();
        errors.AddRange(await rules.ValidateAsync(request, patientId));
        return errors;
    }

    private static void Normalize(PatientWriteDto request)
    {
        // La normalización queda en el mismo DTO para que el futuro guardado use estos valores.
        request.FirstName = request.FirstName?.Trim() ?? string.Empty;
        request.LastName = request.LastName?.Trim() ?? string.Empty;
        request.MiddleName = NormalizeOptional(request.MiddleName);
        request.SecondLastName = NormalizeOptional(request.SecondLastName);
        request.Phone = NormalizeOptional(request.Phone);
        request.Address = NormalizeOptional(request.Address);
        request.Cui = NormalizeOptional(request.Cui);
        request.Occupation = NormalizeOptional(request.Occupation);
        request.ReferredBy = NormalizeOptional(request.ReferredBy);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
