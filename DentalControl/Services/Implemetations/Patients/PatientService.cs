using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Text;
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

    public async Task<Patient?> GetByIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(patientId, 1);
        cancellationToken.ThrowIfCancellationRequested();
        if (userSession.UserId is not int userId || userId <= 0)
            throw new UnauthorizedAccessException("Debes iniciar sesión para consultar pacientes.");
        return await Task.Run(async () =>
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // Un catálogo desactivado conserva su nombre para la consulta histórica.
            return await context.Patients.AsNoTracking().Include(patient => patient.Gender)
                .SingleOrDefaultAsync(patient => patient.Id == patientId && patient.DeletedAt == null &&
                    (patient.Status == EntityStatus.Active || patient.Status == EntityStatus.Inactive), cancellationToken);
        }, cancellationToken);
    }

    public async Task<Patient> UpdateAsync(int patientId, Patient patient, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(patientId, 1);
        ArgumentNullException.ThrowIfNull(patient);
        cancellationToken.ThrowIfCancellationRequested();
        if (userSession.UserId is not int updatedBy || updatedBy <= 0)
            throw new UnauthorizedAccessException("Debes iniciar sesión para editar pacientes.");
        var input = CopyEditableFields(patient);
        businessRules.Validate(input);
        return await Task.Run(async () =>
        {
            for (var attempt = 1; ; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try { return await UpdateAttemptAsync(patientId, input, updatedBy, cancellationToken); }
                catch (Exception ex) when (IsRetryable(ex))
                {
                    if (IsUniqueViolation(ex) && input.Cui is not null &&
                        await CuiExistsAsync(input.Cui, cancellationToken, patientId))
                        throw DuplicateCuiError(input.Cui);
                    if (attempt >= MaxAttempts) throw;
                    await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt), cancellationToken);
                }
            }
        }, cancellationToken);
    }

    private async Task<Patient> UpdateAttemptAsync(int patientId, Patient input, int updatedBy, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var entity = await context.Patients.SingleOrDefaultAsync(patient => patient.Id == patientId &&
            patient.DeletedAt == null && (patient.Status == EntityStatus.Active || patient.Status == EntityStatus.Inactive), cancellationToken)
            ?? throw new KeyNotFoundException("El paciente no existe o ya no está disponible.");
        if (!await catalogService.IsActiveItemAsync(input.GenderId, CategoryCodes.GENDER, cancellationToken))
            throw new ValidationException(new ValidationResult(
                "Selecciona un género activo de la categoría GENDER.", [nameof(Patient.GenderId)]), null, input.GenderId);
        if (input.Cui is not null && await context.Patients.AsNoTracking()
                .AnyAsync(patient => patient.Cui == input.Cui && patient.Id != patientId, cancellationToken))
            throw DuplicateCuiError(input.Cui);

        ApplyEditableFields(input, entity);
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = updatedBy;
        await context.SaveChangesAsync(cancellationToken);
        await context.Entry(entity).Reference(patient => patient.Gender).LoadAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        await transaction.CommitAsync(cancellationToken);
        return entity;
    }

    public async Task<(IReadOnlyList<Patient> Items, int TotalCount, int PageNumber)> GetPageAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchText = null,
        PatientStatusFilter status = PatientStatusFilter.Active,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageNumber, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, 100);
        if (!Enum.IsDefined(status)) throw new ArgumentOutOfRangeException(nameof(status));
        cancellationToken.ThrowIfCancellationRequested();
        if (userSession.UserId is not int userId || userId <= 0)
            throw new UnauthorizedAccessException("Debes iniciar sesión para consultar pacientes.");

        var terms = NormalizeSearchText(searchText)
            .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
            .Distinct(StringComparer.Ordinal).ToArray();

        // Igual que al registrar: SQLite trabaja en un contexto propio fuera del hilo visual.
        return await Task.Run(
            () => ReadPageAsync(pageNumber, pageSize, terms, status, cancellationToken), cancellationToken);
    }

    private async Task<(IReadOnlyList<Patient> Items, int TotalCount, int PageNumber)> ReadPageAsync(
        int pageNumber, int pageSize, string[] terms, PatientStatusFilter status, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.OpenConnectionAsync(cancellationToken);
        var connection = (SqliteConnection)context.Database.GetDbConnection();

        // La función solo pertenece a esta conexión: no altera datos ni el esquema.
        // Se aplica en el WHERE, antes de contar o limitar los resultados.
        connection.CreateFunction<string?, string>("dental_search_normalize", NormalizeSearchText, isDeterministic: true);

        cancellationToken.ThrowIfCancellationRequested();
        // Lectura diferida: total y página comparten una instantánea sin reservar
        // desde el inicio el bloqueo de escritura que requiere el registro.
        await using var transaction = connection.BeginTransaction(deferred: true);
        await using var efTransaction = await context.Database.UseTransactionAsync(transaction, cancellationToken);

        var (sql, parameters) = BuildPatientPageSql(terms, status);
        var totalCount = await context.Patients.FromSqlRaw(sql, parameters).CountAsync(cancellationToken);
        var totalPages = totalCount == 0 ? 1 : (totalCount - 1) / pageSize + 1;
        var effectivePage = Math.Min(pageNumber, totalPages);
        IReadOnlyList<Patient> items = Array.Empty<Patient>();

        if (totalCount > 0)
        {
            var pageParameters = parameters.Concat(new object[] { pageSize, (effectivePage - 1) * pageSize }).ToArray();
            // CAST ordena el consecutivo numéricamente, también después de PAC-999999.
            // Solo se incorporan índices de parámetros, nunca texto escrito por el usuario.
            var pageSql = sql + " ORDER BY CAST(substr(\"patient_code\", 5) AS INTEGER), \"patient_id\"" +
                " LIMIT {" + parameters.Length + "} OFFSET {" + (parameters.Length + 1) + "}";
            items = await context.Patients.FromSqlRaw(pageSql, pageParameters)
                .AsNoTracking().ToListAsync(cancellationToken);
        }

        cancellationToken.ThrowIfCancellationRequested();
        await transaction.CommitAsync(cancellationToken);
        return (items, totalCount, effectivePage);
    }

    private static (string Sql, object[] Parameters) BuildPatientPageSql(string[] terms, PatientStatusFilter status)
    {
        var parameters = new List<object> { EntityStatus.Active, EntityStatus.Inactive };
        var sql = new StringBuilder("SELECT * FROM \"patient\" WHERE \"deleted_at\" IS NULL AND \"status\" IN ({0}, {1})");
        if (status != PatientStatusFilter.All)
        {
            sql.Append(" AND \"status\" = {").Append(parameters.Count).Append('}');
            parameters.Add(status == PatientStatusFilter.Active ? EntityStatus.Active : EntityStatus.Inactive);
        }

        // Cada palabra debe aparecer en los nombres, apellidos o identificadores.
        // instr busca texto literal: % y _ no se interpretan como comodines.
        const string searchableColumns = "coalesce(\"first_name\", '') || ' ' || coalesce(\"middle_name\", '') || ' ' || " +
            "coalesce(\"last_name\", '') || ' ' || coalesce(\"second_last_name\", '') || ' ' || " +
            "coalesce(\"cui\", '') || ' ' || coalesce(\"patient_code\", '')";
        foreach (var term in terms)
        {
            sql.Append(" AND instr(dental_search_normalize(").Append(searchableColumns)
                .Append("), {").Append(parameters.Count).Append("}) > 0");
            parameters.Add(term);
        }
        return (sql.ToString(), parameters.ToArray());
    }

    private static string NormalizeSearchText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var result = new StringBuilder();
        foreach (var character in value.Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                result.Append(character);
        }
        return result.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant();
    }

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

    private async Task<bool> CuiExistsAsync(string cui, CancellationToken cancellationToken, int? excludedId = null)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var query = context.Patients.AsNoTracking().Where(patient => patient.Cui == cui);
        if (excludedId is int id) query = query.Where(patient => patient.Id != id);
        return await query.AnyAsync(cancellationToken);
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


    private static Patient CopyEditableFields(Patient source)
    {
        var target = new Patient();
        ApplyEditableFields(source, target);
        return target;
    }

    private static void ApplyEditableFields(Patient source, Patient target)
    {
        target.FirstName = source.FirstName?.Trim() ?? string.Empty;
        target.MiddleName = NormalizeOptional(source.MiddleName);
        target.LastName = source.LastName?.Trim() ?? string.Empty;
        target.SecondLastName = NormalizeOptional(source.SecondLastName);
        target.BirthDate = source.BirthDate;
        target.GenderId = source.GenderId;
        target.Phone = NormalizeOptional(source.Phone);
        target.Address = NormalizeOptional(source.Address);
        target.Cui = NormalizeOptional(source.Cui);
        target.Occupation = NormalizeOptional(source.Occupation);
        target.ReferredBy = NormalizeOptional(source.ReferredBy);
    }

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
