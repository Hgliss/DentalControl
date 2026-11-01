using System.ComponentModel.DataAnnotations;
using DentalControl.Common;
using DentalControl.Data.Context;
using DentalControl.DTOs.Patients;
using DentalControl.Services.Interface.Catalog;
using DentalControl.Services.Interface.Patients;
using Microsoft.EntityFrameworkCore;

namespace DentalControl.Services.Implemetations.Patients;

/// <summary>Reglas de negocio del paciente; solo consulta datos cuando es necesario.</summary>
public sealed class PatientBusinessRules(
    AppDbContext context, ICatalogService catalogs) : IPatientBusinessRules
{
    public async Task<IReadOnlyList<ValidationResult>> ValidateAsync(
        PatientWriteDto request, int? patientId = null)
    {
        ArgumentNullException.ThrowIfNull(request);
        var errors = new List<ValidationResult>();

        if (request.BirthDate is DateOnly birthDate &&
            birthDate > DateOnly.FromDateTime(DateTime.Today))
        {
            errors.Add(new ValidationResult(
                "La fecha de nacimiento no puede ser futura.",
                [nameof(PatientWriteDto.BirthDate)]));
            return errors;
        }

        if (patientId.HasValue &&
            !await context.Patients.AsNoTracking().AnyAsync(patient => patient.Id == patientId.Value))
        {
            errors.Add(new ValidationResult("El paciente que deseas actualizar no existe."));
            return errors;
        }

        if (!await catalogs.IsActiveItemAsync(request.GenderId.GetValueOrDefault(), CategoryCodes.GENDER))
        {
            errors.Add(new ValidationResult(
                "El género debe ser un ítem activo de la categoría GENDER activa.",
                [nameof(PatientWriteDto.GenderId)]));
        }

        if (!string.IsNullOrEmpty(request.Cui))
        {
            // No filtrar por estado: el documento tampoco puede repetirse en pacientes inactivos.
            var patientsWithCui = context.Patients.AsNoTracking()
                .Where(patient => patient.Cui == request.Cui);

            if (patientId is int currentPatientId)
            {
                patientsWithCui = patientsWithCui.Where(patient => patient.Id != currentPatientId);
            }

            if (await patientsWithCui.AnyAsync())
            {
                errors.Add(new ValidationResult(
                    "Ya existe un paciente registrado con ese CUI.",
                    [nameof(PatientWriteDto.Cui)]));
            }
        }

        return errors;
    }
}
