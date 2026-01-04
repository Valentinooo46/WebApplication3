using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication3.Models;
namespace WebApplication3.SeedData
{


    public class SeedDatabaseJob : IJob
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public SeedDatabaseJob(AppDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (!await _db.Countries.AnyAsync())
            {
                _db.Countries.AddRange(
                    new Country { Id = 1, Name = "Ukraine" },
                    new Country { Id = 2, Name = "Poland" }
                );
            }

            if (!await _db.Cities.AnyAsync())
            {
                _db.Cities.AddRange(
                    new City { Id = 1, Name = "Kyiv", CountryId = 1 },
                    new City { Id = 2, Name = "Lviv", CountryId = 1 },
                    new City { Id = 3, Name = "Warsaw", CountryId = 2 }
                );
            }

            if (!await _db.FlightStatuses.AnyAsync())
            {
                _db.FlightStatuses.AddRange(
                    new FlightStatus { Id = 1, Name = "Scheduled" },
                    new FlightStatus { Id = 2, Name = "Delayed" },
                    new FlightStatus { Id = 3, Name = "Cancelled" },
                    new FlightStatus { Id = 4, Name = "Completed" }
                );
            }

            if (!await _db.Flights.AnyAsync())
            {
                _db.Flights.AddRange(
                    new Flight { Id = 1, Number = "PS101", DepartureCityId = 1, ArrivalCityId = 3, StatusId = 1, AppUserId = "351f33ae-e502-4427-a01f-c9d669ce427d" },
                    new Flight { Id = 2, Number = "LO202", DepartureCityId = 3, ArrivalCityId = 2, StatusId = 2, AppUserId = "9a33e296-e6d5-4041-bb32-c51fb619b72d" }
                );
            }
            var usersFile = Path.Combine(Directory.GetCurrentDirectory(), "SeedData", "Users.json");
            var userDtos = await ReadJsonFileAsync<List<UserSeedDto>>(usersFile);
            if (userDtos != null && userDtos.Count > 0)
            {
                foreach (var dto in userDtos)
                {
                    if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                        continue; // пропускаем некорректные записи

                    var existing = await _userManager.FindByEmailAsync(dto.Email);
                    if (existing != null)
                        continue;

                    var user = new AppUser
                    {
                        UserName = string.IsNullOrWhiteSpace(dto.UserName) ? dto.Email : dto.UserName,
                        Email = dto.Email,
                        FirstName = dto.FirstName ?? "",
                        LastName = dto.LastName ?? "",
                        IconUrl = dto.IconUrl ?? ""
                    };

                    await _userManager.CreateAsync(user, dto.Password);
                    
                }
            }

            await _db.SaveChangesAsync();
        }
        private static async Task<T?> ReadJsonFileAsync<T>(string path) where T : class
        {
            try
            {
                if (!File.Exists(path))
                    return null;
                var json = await File.ReadAllTextAsync(path);
                if (string.IsNullOrWhiteSpace(json))
                    return null;
                var opts = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true
                };
                return JsonSerializer.Deserialize<T>(json, opts);
            }
            catch
            {
                return null;
            }
        }

        private class UserSeedDto
        {
            public string? Email { get; set; }
            public string? Password { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? IconUrl { get; set; }
            public string? UserName { get; set; }
        }
    }
}
