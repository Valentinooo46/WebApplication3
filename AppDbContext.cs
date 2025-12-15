using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;


namespace WebApplication3
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Optional: configure field lengths/indexes
            builder.Entity<AppUser>(e =>
            {
                e.Property(p => p.FirstName).HasMaxLength(128);
                e.Property(p => p.LastName).HasMaxLength(128);
                e.Property(p => p.IconUrl).HasMaxLength(1024);
            });
        }
    }

}
