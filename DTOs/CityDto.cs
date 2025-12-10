namespace WebApplication3.DTOs
{
    public class CityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }

        
        public Guid CountryId { get; set; }
        public string CountryName { get; set; }
        public string CountrySlug { get; set; }
    }
}
