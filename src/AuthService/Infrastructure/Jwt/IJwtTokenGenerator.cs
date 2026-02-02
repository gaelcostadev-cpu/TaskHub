using AuthService.Domain;

namespace AuthService.Infrastructure.Jwt;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user);
}
