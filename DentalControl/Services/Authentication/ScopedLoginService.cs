using Microsoft.Extensions.DependencyInjection;

namespace DentalControl.Services.Authentication;

public sealed class ScopedLoginService(IServiceScopeFactory scopeFactory) : ILoginService
{
    public async Task<AuthenticatedUser?> LoginAsync(string username, string password)
    {
        // Cada intento conserva su propio contexto, incluso si la ventana sigue abierta.
        await using var scope = scopeFactory.CreateAsyncScope();
        var authentication = scope.ServiceProvider.GetRequiredService<AuthenticationService>();
        return await authentication.LoginAsync(username, password);
    }
}
