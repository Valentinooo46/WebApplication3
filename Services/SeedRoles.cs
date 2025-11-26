using Microsoft.AspNetCore.Identity;
namespace WebApplication3.Services;
public class SeedRoles
{
    public static async Task InitializeAsync(RoleManager<IdentityRole> roleManager)
    {
        // Перевіряємо чи існує роль
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }
    }
}