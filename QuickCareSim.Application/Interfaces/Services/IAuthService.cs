

using QuickCareSim.Application.ViewModels.Auth;

namespace QuickCareSim.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterViewModel user);
        Task<LoginResult> LoginAsync(LoginViewModel user);
        Task LogoutAsync();
        Task<bool> ActivateUserAsync(string email, string token);
    }
}
