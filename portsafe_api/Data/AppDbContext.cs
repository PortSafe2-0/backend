using Microsoft.EntityFrameworkCore;
using PortSafe.API.Models;

namespace PortSafe.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Locker> Lockers { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Locker>().HasData(
                new Locker { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000001"), Code = "A01", Location = "Portaria - Bloco A", Status = LockerStatus.Available, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Locker { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000002"), Code = "A02", Location = "Portaria - Bloco A", Status = LockerStatus.Available, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Locker { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000003"), Code = "A03", Location = "Portaria - Bloco A", Status = LockerStatus.Available, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Locker { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000004"), Code = "B01", Location = "Portaria - Bloco B", Status = LockerStatus.Available, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Locker { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000005"), Code = "B02", Location = "Portaria - Bloco B", Status = LockerStatus.Available, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}