using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Controllers
{
    public class CountriesController : Controller
    {
        private readonly CountryDbContext _context;

        public CountriesController(CountryDbContext context)
        {
            _context = context;
        }

        // GET: Countries (Список всех стран)
        public async Task<IActionResult> Index()
        {
            var countries = await _context.Countries.ToListAsync();
            return View(countries);
        }

        // GET: Countries/Details/5 (Детали конкретной страны)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }
    }
}
