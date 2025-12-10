using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;

using System;
using System.Threading.Tasks;
using WebApplication3.Controllers;
using WebApplication3.DTOs;
using WebApplication3.Mappers;
using WebApplication3.Models;
using WebApplication3.Repositories;
using WebApplication3.Services;
using WebApplication3.Validators;

namespace WebApplication3
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddControllersWithViews();

            
            builder.Services.AddHttpClient<ICategoryRepository, CategoryRepository>();

           

            
            builder.Services.AddAutoMapper(typeof(WebApplication3.Mappers.UserProfile), typeof(MappingProfile),typeof(CityProfile));

            
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

            
            builder.Services.AddScoped<IValidator<CountryCreateUpdateDto>, CountryCreateUpdateValidator>();
            builder.Services.AddScoped<IValidator<CityCreateUpdateDto>, CityCreateUpdateValidator>();


            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Countries API", Version = "v1" });
            });

            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactDev", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            var app = builder.Build();

            
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await UserSeeder.SeedAsync(userManager, roleManager);

                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await ProductSeeder.SeedAsync(context);
            }

            
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Countries API v1"));
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            
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

            app.UseCors("AllowReactDev");

            app.UseAuthentication();
            app.UseAuthorization();

           
            app.MapControllers();

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