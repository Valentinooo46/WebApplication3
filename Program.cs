using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using System;
using System.Text;
using System.Threading.Tasks;
using WebApplication3.Controllers;
using WebApplication3.Models;
using WebApplication3.Services;
using Microsoft.AspNetCore.Authentication;


namespace WebApplication3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddControllersWithViews();

            
          

           

            
            
            
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Identity
            builder.Services.AddIdentity<AppUser, IdentityRole>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                // Dev-friendly passwords; tighten for prod
                opts.Password.RequiredLength = 6;
                opts.Password.RequireDigit = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();


            builder.Services.AddHttpClient();


            builder.Services.Configure<GoogleSettings>(builder.Configuration.GetSection("Google"));
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Countries API", Version = "v1" });
            });

            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactDev", policy =>
                {
                    policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
            });
            var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "WebApplication3";
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "WebApplication3.Web";
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            // Authentication (Identity cookies + JWT + Google)
            // Set default scheme to JWT for API calls; Identity uses cookies internally for external flows
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // enable true in prod
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = signingKey,
                    ClockSkew = TimeSpan.FromMinutes(2)
                };
            });
            

            builder.Services.AddAuthorization();

            // ... (інші using та конфігурації лишаються без змін)

            var app = builder.Build();

            // Виконання міграцій тощо...

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

            // 1) Спочатку роутинг
            app.UseRouting();

            // 2) Потім CORS (після UseRouting, ДО Authentication/Authorization)
            app.UseCors("AllowReactDev");

            // 3) Далі аутентифікація/авторизація
            app.UseAuthentication();
            app.UseAuthorization();

            // 4) І потім мапінг контролерів (endpoints)
            app.MapControllers();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // ... інші служби/запуски
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