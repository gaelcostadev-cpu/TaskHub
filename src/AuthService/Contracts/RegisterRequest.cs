using System.ComponentModel.DataAnnotations;

namespace AuthService.Contracts
{
    public class RegisterRequest
    {
        public string Email { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
