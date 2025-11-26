using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using WebApplication3.Controllers;
using WebApplication3.Models;
using WebApplication3.Repositories;
using WebApplication3.Services;


namespace WebApplication3
{
    public class Program
    {
        
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;

                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            builder.Services.AddScoped<UserManager<ApplicationUser>>();
            builder.Services.AddScoped<SignInManager<ApplicationUser>>();
            builder.Services.AddScoped<RoleManager<IdentityRole>>();
            builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();




            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await SeedRoles.InitializeAsync(roleManager);
            }



            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                
            }
            
            

            // Middleware для паузи
            app.Use(async (context, next) =>
            {
                if (ServerState.IsPaused)
                {
                    context.Response.StatusCode = 503;
                    await context.Response.WriteAsync("Сервер на паузі. Спробуйте пізніше.");
                }
                else
                {
                    await next();
                }
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
            Task.Run(() => CheckInputKey(lifetime));
            
            app.Run();
        }

        private static void CheckInputKey(IHostApplicationLifetime lifetime)
        {
            Console.WriteLine("Press 'q' to quit, 'p' to pause/resume, any other key to continue...");
            while (true)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Q)
                {
                    //Console.WriteLine($"Додано нових записів:{HomeController.count}");
                    lifetime.StopApplication();
                    break;
                }
                else if (key == ConsoleKey.P)
                {
                    ServerState.IsPaused = !ServerState.IsPaused;
                    Console.WriteLine(ServerState.IsPaused ? "Сервер на паузі." : "Сервер відновлено.");
                }
                else
                {
                    Console.WriteLine($"You pressed {key}!");
                }
            }
        }
    }
    public static class ServerState
    {
        public static volatile bool IsPaused = false;
    }

}
