using Microsoft.AspNetCore.Identity;

namespace WebApplication3.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Виправлені імені властивостей до PascalCase
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public int Age { get; set; }

        // IdentityUser вже містить PhoneNumber, не дублюємо його

        // Додаємо FullName — використовується в AccountController
        public string? FullName { get; set; }
    }
}
