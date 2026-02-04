namespace AuthService.Contracts
{
    public class LoginResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
        public string AccessToken { get; set; } = null!;
        public int ExpiresIn { get; set; } //segundos
    }
}
