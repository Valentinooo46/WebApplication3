namespace WebApplication3.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<City> Cities { get; set; }
    }

    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public ICollection<AppUser> AppUsers { get; set; }
    }

    public class FlightStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Flight
    {
        public int Id { get; set; }
        public string Number { get; set; }

        public int DepartureCityId { get; set; }
        public City DepartureCity { get; set; }

        public int ArrivalCityId { get; set; }
        public City ArrivalCity { get; set; }

        public int StatusId { get; set; }
        public FlightStatus Status { get; set; }

        public string AppUserId { get; set; } // Identity ключ — string
        public AppUser AppUser { get; set; }
    }
}
