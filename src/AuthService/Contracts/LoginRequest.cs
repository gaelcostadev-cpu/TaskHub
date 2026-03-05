using System.ComponentModel.DataAnnotations;

namespace AuthService.Contracts
{
    public class LoginRequest
    {
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
