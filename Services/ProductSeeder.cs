using WebApplication3.Models;

namespace WebApplication3.Services
{
    public static class ProductSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!context.Products.Any())
            {
                var products = new List<Product>
            {
                new Product
                {
                    Name = "Ноутбук Lenovo ThinkPad",
                    Description = "Надійний бізнес-ноутбук з клавіатурою ThinkPad та потужним процесором.",
                    Images = new List<ProductImage>
                    {
                        new ProductImage { Url = "https://example.com/images/thinkpad1.jpg" },
                        new ProductImage { Url = "https://example.com/images/thinkpad2.jpg" }
                    }
                },
                new Product
                {
                    Name = "Смартфон Samsung Galaxy",
                    Description = "Сучасний смартфон з AMOLED дисплеєм та потужною камерою.",
                    Images = new List<ProductImage>
                    {
                        new ProductImage { Url = "https://example.com/images/galaxy1.jpg" },
                        new ProductImage { Url = "https://example.com/images/galaxy2.jpg" },
                        new ProductImage { Url = "https://example.com/images/galaxy3.jpg" }
                    }
                },
                new Product
                {
                    Name = "Навушники Sony WH-1000XM4",
                    Description = "Бездротові навушники з шумопоглинанням та високою якістю звуку.",
                    Images = new List<ProductImage>
                    {
                        new ProductImage { Url = "https://example.com/images/sony1.jpg" },
                        new ProductImage { Url = "https://example.com/images/sony2.jpg" }
                    }
                }
            };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
