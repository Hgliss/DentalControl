using DentalControl.Data.Context;
using DentalControl.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DentalControl.Data.Database.Common;

namespace DentalControl.Services.Authentication;

/// <summary>
/// Valida las credenciales para acceder al sistema.
/// </summary>
public class AuthenticationService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthenticationService(
        AppDbContext context,
        IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthenticatedUser?> LoginAsync(
        string username,
        string password)
    {
        if(string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var normalizedUsername = username.Trim();

        var user = await _context.Users
            .Include(user => user.Role)
            .SingleOrDefaultAsync(
                user => user.Username == normalizedUsername);

        if(user is null ||
            user.Status != EntityStatus.Active ||
            user.Role is null ||
            user.Role.Status != EntityStatus.Active)
        {
            return null;
        }

        var verificationResult =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (verificationResult ==
            PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = 
                _passwordHasher.HashPassword(user, password);

            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = user.Id;   

            await _context.SaveChangesAsync();
        }

        return new AuthenticatedUser(
            user.Id,
            user.Username,
            $"{user.FirstName} {user.LastName}",
            user.Role.Name);

    }

}
