using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;

namespace WebApplication3
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Твої власні DbSet-и
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Country>()
                .HasIndex(c => c.Name)
                .IsUnique();

            builder.Entity<Country>()
                .HasIndex(c => c.Code)
                .IsUnique();

            builder.Entity<Country>()
                .HasIndex(c => c.Slug)
                .IsUnique();




            builder.Entity<City>()
                .HasIndex(c => new { c.CountryId, c.Name })
                .IsUnique();

            builder.Entity<City>()
                .HasIndex(c => new { c.CountryId, c.Slug })
                .IsUnique();

            // Один Country має багато City (one-to-many)
            builder.Entity<Country>()
                .HasMany(c => c.Cities)
                .WithOne(c => c.Country)
                .HasForeignKey(c => c.CountryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
