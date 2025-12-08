//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using WebApplication3.Models;

//namespace WebApplication3.Services
//{
//    public class DataSeederHostedService : IHostedService
//    {
//        private readonly IServiceProvider _serviceProvider;
//        private readonly IWebHostEnvironment _env;

//        public DataSeederHostedService(IServiceProvider serviceProvider, IWebHostEnvironment env)
//        {
//            _serviceProvider = serviceProvider;
//            _env = env;
//        }

//        public async Task StartAsync(CancellationToken cancellationToken)
//        {
//            using (var scope = _serviceProvider.CreateScope())
//            {
//                var context = scope.ServiceProvider.GetRequiredService<CountryDbContext>();
//                var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

//                await context.Database.MigrateAsync(cancellationToken); 

//                if (!context.Countries.Any())
//                {
//                    await SeedCountriesAsync(context, httpClientFactory, cancellationToken);
//                }
//            }
//        }

//        private async Task SeedCountriesAsync(CountryDbContext context, IHttpClientFactory httpClientFactory, CancellationToken cancellationToken)
//        {
//            var jsonFilePath = Path.Combine(_env.ContentRootPath, "SeedData", "countries.json");
//            var json = await File.ReadAllTextAsync(jsonFilePath, cancellationToken);
//            var countries = JsonConvert.DeserializeObject<List<Country>>(json);

//            var httpClient = httpClientFactory.CreateClient();
//            var wwwrootPath = Path.Combine(_env.WebRootPath, "images");
//            Directory.CreateDirectory(wwwrootPath);

//            foreach (var country in countries!)
//            {
//                // Download image
//                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(country.ImageUrl)}";
//                var imagePath = Path.Combine(wwwrootPath, imageName);
//                var imageUrl = country.ImageUrl;

//                try
//                {
//                    var imageBytes = await httpClient.GetByteArrayAsync(imageUrl, cancellationToken);
//                    await File.WriteAllBytesAsync(imagePath, imageBytes, cancellationToken);
//                    country.ImagePath = $"/images/{imageName}"; 
//                    country.ImageUrl = imageUrl; 
//                }
//                catch (HttpRequestException ex)
//                {
                    
//                    Console.WriteLine($"Error downloading image from {imageUrl}: {ex.Message}");
//                    country.ImagePath = "/images/placeholder.jpg"; 
//                }

//                context.Countries.Add(country);
//            }

//            await context.SaveChangesAsync(cancellationToken);
//        }

//        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
//    }
//}
