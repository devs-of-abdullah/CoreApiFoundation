using Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        public DbSet<UserEntity> users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.HasIndex(u => u.Email)
                      .IsUnique();

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                entity.Property(u => u.Role)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(u => u.RefreshTokenHash)
                      .HasMaxLength(256);

                entity.Property(u => u.RefreshTokenExpiresAt);
                     

                entity.HasIndex(u => u.RefreshTokenHash)
                      .IsUnique();

                entity.HasQueryFilter(u => !u.IsDeleted);
            });

            
        }


    }
}
