namespace AuthService.Domain
{
    public class User
    {
        public Guid Id { get; private set; }

        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }

        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiresAt { get; private set; }

        public DateTime CreatedAt { get; private set; }

        protected User()
        {
            Username = null!;
            Email = null!;
            PasswordHash = null!;
        }

        public User(string username, string email, string passwordHash)
        {
            Id = Guid.NewGuid();
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            CreatedAt = DateTime.UtcNow;
        }

        public void SetRefreshToken(string token, DateTime expiresAt)
        {
            RefreshToken = token;
            RefreshTokenExpiresAt = expiresAt;
        }

        public void RevokeRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiresAt = null;
        }

        public bool IsRefreshTokenValid(string token)
        {
            return RefreshToken == token &&
                   RefreshTokenExpiresAt.HasValue &&
                   RefreshTokenExpiresAt > DateTime.UtcNow;
        }
    }
}
