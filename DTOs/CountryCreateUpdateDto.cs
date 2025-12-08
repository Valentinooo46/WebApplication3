using System.ComponentModel.DataAnnotations;

namespace WebApplication3.DTOs
{
    public class CountryCreateUpdateDto
    {
        public Guid? Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneCode { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Slug { get; set; }
        public string Description { get; set; }
        public string FlagUrl { get; set; }
    }
}
