using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using WebApplication3.Models;

namespace WebApplication3
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<FlightStatus> FlightStatuses { get; set; }
        public DbSet<Flight> Flights { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // AppUser config
            builder.Entity<AppUser>(e =>
            {
                e.Property(p => p.FirstName).HasMaxLength(128);
                e.Property(p => p.LastName).HasMaxLength(128);
                e.Property(p => p.IconUrl).HasMaxLength(1024);


            });
            
                
            // Country → City
            builder.Entity<City>()
                .HasOne(c => c.Country)
                .WithMany(cn => cn.Cities)
                .HasForeignKey(c => c.CountryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Flight → Status
            builder.Entity<Flight>()
                .HasOne(f => f.Status)
                .WithMany()
                .HasForeignKey(f => f.StatusId)
                .OnDelete(DeleteBehavior.Cascade);

            // Flight → AppUser
            builder.Entity<Flight>()
                .HasOne(f => f.AppUser)
                .WithMany()
                .HasForeignKey(f => f.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Flight → Cities
            builder.Entity<Flight>()
                .HasOne(f => f.DepartureCity)
                .WithMany()
                .HasForeignKey(f => f.DepartureCityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Flight>()
                .HasOne(f => f.ArrivalCity)
                .WithMany()
                .HasForeignKey(f => f.ArrivalCityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}