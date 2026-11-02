using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using DentalControl.Common;
using DentalControl.Data.Context;
using DentalControl.Data.Database.Common;
using DentalControl.Models.Entities;
using DentalControl.Services.Interface.Catalog;
using DentalControl.Services.Interface.Authentication;
using DentalControl.Services.Interface.Patients;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DentalControl.Services.Implemetations.Patients;

/// <summary>
/// Coordina las reglas y la persistencia del registro de pacientes.
/// </summary>
public sealed class PatientService(
    IServiceScopeFactory scopeFactory,
    PatientBusinessRules businessRules,
    ICatalogService catalogService,
    IUserSession userSession) : IPatientService
{
    private const int MaxAttempts = 3;

    public async Task<Patient> CreateAsync(
        Patient patient, CancellationToken 
        cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(patient);

        cancellationToken.ThrowIfCancellationRequested();

        // Capturar la identidad una vez: todos los reintentos pertenecen al mismo usuario.
        if (userSession.UserId is not int createdBy || createdBy <= 0)
            throw new UnauthorizedAccessException("Debes iniciar sesión para registrar pacientes.");

        // Copia solo los campos del formulario
        var input = CopyEditableFields(patient);
        businessRules.Validate(input);

        // Microsoft.Data.Sqlite ejecuta sus operaciones nativas de forma síncrona.
        // Todo contexto se crea dentro del trabajo, sin compartirlo con la interfaz.
        return await Task.Run(() => CreateValidatedAsync(input, createdBy, cancellationToken), cancellationToken);
    }

    private async Task<Patient> CreateValidatedAsync(Patient input, int createdBy, CancellationToken cancellationToken)
    {
        for (var attempt = 1; ; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return await CreateAttemptAsync(input, createdBy, cancellationToken);
            }
            catch (Exception ex) when (IsRetryable(ex))
            {
                // La transacción y el contexto del intento ya se liberaron y revirtieron.
                if (IsUniqueViolation(ex) && input.Cui is not null &&
                    await CuiExistsAsync(input.Cui, cancellationToken))
                    throw DuplicateCuiError(input.Cui);

                if (attempt >= MaxAttempts)
                    throw;

                await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt), cancellationToken);
            }
        }
    }

    private async Task<Patient> CreateAttemptAsync(Patient input, int createdBy, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // SQLite inicia una transacción de escritura no diferida: el consecutivo
        // se consulta y se guarda bajo el mismo bloqueo, incluso entre instancias.
        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);

        if (!await catalogService.IsActiveItemAsync(input.GenderId, CategoryCodes.GENDER, cancellationToken))
            throw new ValidationException(new ValidationResult(
                "Selecciona un género activo de la categoría GENDER.", [nameof(Patient.GenderId)]), null, input.GenderId);

        if (input.Cui is not null && await context.Patients.AsNoTracking()
                .AnyAsync(existing => existing.Cui == input.Cui, cancellationToken))
            throw DuplicateCuiError(input.Cui);

        var entity = CopyEditableFields(input);
        entity.PatientCode = await GenerateCodeAsync(context, cancellationToken);
        entity.Status = EntityStatus.Active;
        entity.CreatedAt = DateTime.UtcNow;
        entity.CreatedBy = createdBy;
        // No se acepta auditoría, identificadores ni relaciones del objeto recibido.
        context.Patients.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        await transaction.CommitAsync(cancellationToken);
        return entity;
    }

    private async Task<bool> CuiExistsAsync(string cui, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await context.Patients.AsNoTracking()
            .AnyAsync(patient => patient.Cui == cui, cancellationToken);
    }

    private static async Task<string> GenerateCodeAsync(AppDbContext context, CancellationToken cancellationToken)
    {
        // Incluye todos los estados. Comparar sufijos numéricos evita errores al superar seis dígitos.
        var codes = await context.Patients.AsNoTracking()
            .Select(patient => patient.PatientCode).ToListAsync(cancellationToken);
        long maximum = 0;
        foreach (var code in codes)
        {
            if (code is null || code.Length is < 10 or > 20 ||
                !code.StartsWith("PAC-", StringComparison.Ordinal) ||
                code.AsSpan(4).ContainsAnyExceptInRange('0', '9') ||
                !long.TryParse(code.AsSpan(4), NumberStyles.None, CultureInfo.InvariantCulture, out var number) ||
                number <= 0)
                throw new ValidationException("Hay códigos de pacientes con formato inválido. Revísalos antes de registrar.");

            maximum = Math.Max(maximum, number);
        }

        var nextCode = "PAC-" + (maximum + 1).ToString("D6", CultureInfo.InvariantCulture);
        if (nextCode.Length > 20)
            throw new ValidationException("Se alcanzó el límite de numeración de pacientes.");
        return nextCode;
    }


    private static Patient CopyEditableFields(Patient source) => new()
    {
        FirstName = source.FirstName?.Trim() ?? string.Empty,
        MiddleName = NormalizeOptional(source.MiddleName),
        LastName = source.LastName?.Trim() ?? string.Empty,
        SecondLastName = NormalizeOptional(source.SecondLastName),
        BirthDate = source.BirthDate,
        GenderId = source.GenderId,
        Phone = NormalizeOptional(source.Phone),
        Address = NormalizeOptional(source.Address),
        Cui = NormalizeOptional(source.Cui),
        Occupation = NormalizeOptional(source.Occupation),
        ReferredBy = NormalizeOptional(source.ReferredBy)
    };

    private static ValidationException DuplicateCuiError(string cui) => new(
        new ValidationResult("Ya existe un paciente con ese CUI.", [nameof(Patient.Cui)]), null, cui);

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static bool IsRetryable(Exception exception) =>
        GetSqliteException(exception) is { SqliteErrorCode: 5 or 6 } || IsUniqueViolation(exception);

    private static bool IsUniqueViolation(Exception exception) =>
        GetSqliteException(exception) is { SqliteExtendedErrorCode: 2067 };

    private static SqliteException? GetSqliteException(Exception exception) => exception switch
    {
        SqliteException sqlite => sqlite,
        DbUpdateException { InnerException: SqliteException sqlite } => sqlite,
        _ => null
    };
}
