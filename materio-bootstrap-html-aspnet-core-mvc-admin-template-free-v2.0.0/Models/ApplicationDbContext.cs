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

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
      builder.Entity<AppUser>()
        .ToTable("AspNetUsers");
      // Додаткові налаштування при необхідності
    }
  }
}
