using AuthService.Contracts;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Jwt;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthApplicationService _authApplicationService;
        private readonly AuthDbContext _dbContext;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly JwtSettings _jwtSettings;

        public AuthController(IUserService userService, IAuthApplicationService authApplicationService, 
            AuthDbContext dbContext, IJwtTokenGenerator jwtTokenGenerator, IOptions<JwtSettings> jwtOptions)
        {
            _userService = userService;
            _dbContext = dbContext;
            _authApplicationService = authApplicationService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _jwtSettings = jwtOptions.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _userService.RegisterAsync(request);
                return CreatedAtAction(nameof(Register), result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var result = await _authApplicationService.LoginAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

            if (user == null || !user.IsRefreshTokenValid(request.RefreshToken))
                return Unauthorized();

            var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(user);

            return Ok(new
            {
                AccessToken = newAccessToken,
                ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60
            });
        }

    }
}
