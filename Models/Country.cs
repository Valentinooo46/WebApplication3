using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class Country
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        // телефонний код, наприклад "+380"
        [Required]
        [MaxLength(10)]
        public string PhoneCode { get; set; }

        // короткий унікальний код країни, наприклад "UA"
        [Required]
        [MaxLength(10)]
        public string Code { get; set; }

        // для дружніх URL, унікальний
        [Required]
        [MaxLength(200)]
        public string Slug { get; set; }

        public string Description { get; set; }

        // зберігаємо URL або шлях до зображення прапора
        public string FlagUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
