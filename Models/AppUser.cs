using Microsoft.AspNetCore.Identity;

namespace WebApplication3.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string IconUrl { get; set; } = "";
    }

}
