using AuthService.Domain;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure {
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var user = modelBuilder.Entity<User>();

            user.ToTable("Users");

            user.HasKey(u => u.Id);

            user.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            user.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            user.Property(u => u.PasswordHash)
                .IsRequired();

            user.Property(u => u.RefreshToken)
                .HasMaxLength(512);

            user.Property(u => u.CreatedAt)
                .IsRequired();

            user.HasIndex(u => u.Email)
                .IsUnique();

            user.HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}
