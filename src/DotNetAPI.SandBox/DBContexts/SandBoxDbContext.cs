using Microsoft.EntityFrameworkCore;

namespace DotNetAPI.SandBox.DBContexts
{
    public class SandBoxDbContext : DbContext
    {
        public SandBoxDbContext()
        {
        }

        public SandBoxDbContext(DbContextOptions<SandBoxDbContext> options) : base(options)
        {
        }

        public virtual DbSet<User>? User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => 
            {
                entity.ToTable("User");

                entity.HasKey(e => e.UserId).HasName("PK_User");
                entity.Property(e => e.UserId).HasColumnName("UserId").HasDefaultValueSql("(newid())");
                entity.Property(e => e.Username).HasColumnName("Username").HasMaxLength(50);

            });
        }
    }

    public class User
    {
        public Guid UserId { get; set; }
        public string? Username { get; set; }
    }
}
