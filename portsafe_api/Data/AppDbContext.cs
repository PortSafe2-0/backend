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

        // No futuro: + DbSet<Locker> e DbSet<Delivery>
    }
}