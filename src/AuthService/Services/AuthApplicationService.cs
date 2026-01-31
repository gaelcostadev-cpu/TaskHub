using AuthService.Contracts;
using AuthService.Domain;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Jwt;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services
{
    public class AuthApplicationService : IAuthApplicationService
    {
        private readonly AuthDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthApplicationService(AuthDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public AuthApplicationService(AuthDbContext dbContext,IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }


        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var normalizedEmail = request.Email.ToLower();

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == normalizedEmail);

            if (user is null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var isPasswordValid = _passwordHasher.Verify(
                   request.Password,
                   user.PasswordHash
               );

            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);

            return new LoginResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Username = user.Username,
                AccessToken = accessToken,
                ExpiresIn = 60 * 15 // 15 min
            };

        }
    }
}
