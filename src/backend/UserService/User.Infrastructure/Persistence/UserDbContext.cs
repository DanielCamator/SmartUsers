using Microsoft.EntityFrameworkCore;

namespace User.Infrastructure.Persistence;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    public DbSet<Domain.Entities.User> Users => Set<Domain.Entities.User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User.Domain.Entities.User>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Email).IsRequired().HasMaxLength(150);
            b.HasIndex(u => u.Email).IsUnique();
        });
    }
}