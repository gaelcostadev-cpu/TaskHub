using AuthService.Contracts;
using AuthService.Domain;
using AuthService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services
{
    public class UserService : IUserService
    {
        private readonly AuthDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(AuthDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == request.Email);

            if (emailExists)
                throw new InvalidOperationException("Email já está em uso.");

            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username == request.Username);

            if (usernameExists)
                throw new InvalidOperationException("Username já está em uso.");

            var passwordHash = _passwordHasher.Hash(request.Password);

            var user = new User(
                request.Username,
                request.Email,
                passwordHash
            );

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new RegisterResponse
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username
            };
        }
    }
}
