using AuthService.Contracts;

namespace AuthService.Services;

public interface IAuthApplicationService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}

