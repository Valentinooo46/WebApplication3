using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AspnetCoreMvcFull.Models
{
  public class ApplicationDbContext : IdentityDbContext<AppUser>
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Country> Countries => Set<Country>();
    public DbSet<City> Cities => Set<City>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<AppUser>()
        .ToTable("AspNetUsers");

      builder.Entity<Country>(entity =>
      {
        entity.ToTable("Countries");
        entity.Property(e=> e.Id).UseIdentityColumn();
        entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        entity.HasIndex(e => e.Name).IsUnique();
      });

      builder.Entity<City>(entity =>
      {
        entity.ToTable("Cities");
        entity.Property(e=> e.Id).UseIdentityColumn();
        entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        entity.HasIndex(e => new
        {
          e.CountryId,
          e.Name
        }).IsUnique();
        entity.HasOne(e => e.Country)
                      .WithMany(c => c.Cities)
                      .HasForeignKey(e => e.CountryId)
                      .OnDelete(DeleteBehavior.Cascade);
      });
    }
  }
}
