using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication3.Models
{
    [Table("Cities2")]
    public class City
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

       
        [Required]
        [MaxLength(200)]
        public string Slug { get; set; }

        public string Description { get; set; }

        
        [Required]
        public Guid CountryId { get; set; }

        [ForeignKey(nameof(CountryId))]
        public Country Country { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
