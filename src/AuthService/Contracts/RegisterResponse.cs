namespace AuthService.Contracts
{
    public class RegisterResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
    }
}
