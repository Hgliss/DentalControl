using DentalControl.Data.Context;
using DentalControl.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DentalControl.Data.initialization;

/// <summary>
/// Prepara los roles y el usuario administrador inicial.
/// </summary>
public class DatabaseInitializer
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public DatabaseInitializer(
        AppDbContext context,
        IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Crea los registros iniciales que faltan sin modificar los existentes.
    /// </summary>
    public async Task InitializeAsync()
    {
        const string roleName = "Administrador";
        const string username = "admin";

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var administratorRole = await _context.Roles
            .SingleOrDefaultAsync(role => role.Name == roleName);

        if (administratorRole is null)
        {
            administratorRole = new Role
            {
                Name = roleName,
                Description = "Administración general del sistema.",
                Status = "A"
            };

            _context.Roles.Add(administratorRole);
        }

        var doctorRoleExists = await _context.Roles
            .AnyAsync(role => role.Name == "Doctor");

                if (!doctorRoleExists)
                {
                    var doctorRole = new Role
                    {
                        Name = "Doctor",
                        Description = "Atención clínica y gestión de pacientes.",
                        Status = "A"
                    };

                    _context.Roles.Add(doctorRole);
        }


        var administratorExists = await _context.Users
            .AnyAsync(user => user.Username == username);

        if (!administratorExists)
        {
            if (administratorRole.Status != "A")
            {
                throw new InvalidOperationException(
                    "El rol Administrador existe, pero no está activo.");
            }

            var password = Environment.GetEnvironmentVariable(
                "DENTALCONTROL_INITIAL_ADMIN_PASSWORD",
                EnvironmentVariableTarget.User);

            if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
            {
                throw new InvalidOperationException(
                    "Configura DENTALCONTROL_INITIAL_ADMIN_PASSWORD " +
                    "con una contraseña de al menos 12 caracteres.");
            }

            var administrator = new User
            {
                Username = username,
                FirstName = "Administrador",
                LastName = "Inicial",
                Role = administratorRole,
                Status = "A"
            };

            administrator.PasswordHash =
                _passwordHasher.HashPassword(administrator, password);

            _context.Users.Add(administrator);
        }

        var doctorExists = await _context.Users
    .AnyAsync(user => user.Username == "doctor");

        if (!doctorExists)
        {
            var doctorRole = _context.Roles.Local
                .SingleOrDefault(role => role.Name == "Doctor")
                ?? await _context.Roles
                    .SingleAsync(role => role.Name == "Doctor");

            if (doctorRole.Status != "A")
            {
                throw new InvalidOperationException(
                    "El rol Doctor existe, pero no está activo.");
            }

            var password = Environment.GetEnvironmentVariable(
                "DENTALCONTROL_INITIAL_DOCTOR_PASSWORD",
                EnvironmentVariableTarget.User);

            if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
            {
                throw new InvalidOperationException(
                    "Configura DENTALCONTROL_INITIAL_DOCTOR_PASSWORD " +
                    "con una contraseña de al menos 12 caracteres.");
            }

            var doctor = new User
            {
                Username = "doctor",
                FirstName = "Doctor",
                LastName = "Prueba",
                Role = doctorRole,
                Status = "A"
            };

            doctor.PasswordHash =
                _passwordHasher.HashPassword(doctor, password);

            _context.Users.Add(doctor);
        }


        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}