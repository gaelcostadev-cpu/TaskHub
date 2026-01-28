namespace AuthService.Contracts
{
    public class LoginResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
    }
}
