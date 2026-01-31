using System.ComponentModel.DataAnnotations;

namespace AuthService.Contracts
{
    public class HashRequest
    {
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
