using Bogus;
using Microsoft.AspNetCore.Identity;
using WebApplication3.Models;

namespace WebApplication3.Services
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles.InitializeAsync(roleManager);
            
            if (!userManager.Users.Any())
            {
                var faker = new Faker("uk");

                
                for (int i = 0; i < 2; i++)
                {
                    var adminUser = new ApplicationUser
                    {
                        UserName = faker.Internet.UserName(),
                        Email = faker.Internet.Email(),
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(adminUser, "Admin123!"); 
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        await userManager.AddToRoleAsync(adminUser, "User");
                    }
                }

                
                for (int i = 0; i < 3; i++)
                {
                    var normalUser = new ApplicationUser
                    {
                        UserName = faker.Internet.UserName(),
                        Email = faker.Internet.Email(),
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(normalUser, "User123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(normalUser, "User");
                    }
                }
            }
        }
    }

}
