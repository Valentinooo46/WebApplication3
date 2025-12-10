namespace WebApplication3.DTOs
{
    public class CityCreateUpdateDto
    {
        public Guid? Id { get; set; } 

        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }

        
        public string CountrySlug { get; set; }
    }
}
