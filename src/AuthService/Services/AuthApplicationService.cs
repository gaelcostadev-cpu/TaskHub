using AuthService.Contracts;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Jwt;
using AuthService.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuthService.Services
{
    public class AuthApplicationService : IAuthApplicationService
    {
        private readonly AuthDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly JwtSettings _jwtSettings;

        public AuthApplicationService(AuthDbContext dbContext,IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator, IRefreshTokenGenerator refreshTokenGenerator,
            IOptions<JwtSettings> jwtOptions)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var normalizedEmail = request.Email.ToLower();

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail) 
                ?? throw new UnauthorizedAccessException("Invalid credentials");

            var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid) throw new UnauthorizedAccessException("Invalid credentials");
            

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
            var refreshToken = _refreshTokenGenerator.Generate();
            var refreshExpiresAt = DateTime.UtcNow.AddDays(7);

            user.SetRefreshToken(refreshToken, refreshExpiresAt);

            await _dbContext.SaveChangesAsync();

            return new LoginResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Username = user.Username,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60
            };

        }


    }
}
