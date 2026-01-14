using AuthService.Contracts;

namespace AuthService.Services
{
    public interface IUserService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    }
}
