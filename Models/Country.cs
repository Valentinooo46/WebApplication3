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

        
        [Required]
        [MaxLength(10)]
        public string PhoneCode { get; set; }

        [Required]
        [MaxLength(10)]
        public string Code { get; set; }

        
        [Required]
        [MaxLength(200)]
        public string Slug { get; set; }

        public string Description { get; set; }

        
        public string FlagUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<City> Cities { get; set; }
    }
}
