namespace WebApplication3.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public string ImageUrl { get; set; } // URL to download from
        public string ImagePath { get; set; } // Local path to saved image
    }
}
