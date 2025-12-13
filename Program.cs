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