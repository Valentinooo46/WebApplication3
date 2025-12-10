using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using WebApplication3.DTOs;
using WebApplication3.Models;
using Ganss.Xss;

namespace WebApplication3.Controllers
{
    public class CitiesMvcController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IValidator<CityCreateUpdateDto> _validator;

        public CitiesMvcController(AppDbContext db, IMapper mapper, IValidator<CityCreateUpdateDto> validator)
        {
            _db = db;
            _mapper = mapper;
            _validator = validator;
        }

        // GET: /CitiesMvc
        public async Task<IActionResult> Index()
        {
            var cities = await _db.Cities.Include(c => c.Country).OrderBy(c => c.Name).ToListAsync();
            var vm = _mapper.Map<CityDto[]>(cities);
            return View(vm);
        }

        // GET: /CitiesMvc/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var city = await _db.Cities.Include(c => c.Country).FirstOrDefaultAsync(c => c.Id == id);
            if (city == null) return NotFound();
            var vm = _mapper.Map<CityDto>(city);
            return View(vm);
        }

        // GET: /CitiesMvc/Create
        public async Task<IActionResult> Create()
        {
            await PopulateCountriesSelectList();
            return View(new CityCreateUpdateDto());
        }

        // POST: /CitiesMvc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CityCreateUpdateDto dto)
        {
            dto.Id = null;

            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                foreach (var err in validation.Errors)
                    ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
                await PopulateCountriesSelectList();
                return View(dto);
            }

            var country = await _db.Countries.SingleOrDefaultAsync(c => c.Slug == dto.CountrySlug);
            if (country == null)
            {
                ModelState.AddModelError(nameof(dto.CountrySlug), "Country not found.");
                await PopulateCountriesSelectList();
                return View(dto);
            }

            
            var sanitizer = new HtmlSanitizer();
            var safeHtml = sanitizer.Sanitize(dto.Description ?? string.Empty);

            var entity = _mapper.Map<City>(dto);
            entity.Id = Guid.NewGuid();
            entity.CountryId = country.Id;
            entity.Description = safeHtml;

            _db.Cities.Add(entity);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: /CitiesMvc/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var city = await _db.Cities.Include(c => c.Country).FirstOrDefaultAsync(c => c.Id == id);
            if (city == null) return NotFound();

            var dto = _mapper.Map<CityCreateUpdateDto>(city);
            // ensure dto.CountrySlug populated
            dto.CountrySlug = city.Country?.Slug;
            await PopulateCountriesSelectList();
            return View(dto);
        }

        // POST: /CitiesMvc/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CityCreateUpdateDto dto)
        {
            dto.Id = id;
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                foreach (var err in validation.Errors)
                    ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
                await PopulateCountriesSelectList();
                return View(dto);
            }

            var existing = await _db.Cities.FindAsync(id);
            if (existing == null) return NotFound();

            var country = await _db.Countries.SingleOrDefaultAsync(c => c.Slug == dto.CountrySlug);
            if (country == null)
            {
                ModelState.AddModelError(nameof(dto.CountrySlug), "Country not found.");
                await PopulateCountriesSelectList();
                return View(dto);
            }

            var sanitizer = new HtmlSanitizer();
            var safeHtml = sanitizer.Sanitize(dto.Description ?? string.Empty);

            _mapper.Map(dto, existing);
            existing.CountryId = country.Id;
            existing.Description = safeHtml;
            existing.Id = id;

            _db.Cities.Update(existing);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /CitiesMvc/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var city = await _db.Cities.Include(c => c.Country).FirstOrDefaultAsync(c => c.Id == id);
            if (city == null) return NotFound();

            var vm = _mapper.Map<CityDto>(city);
            return View(vm);
        }

        // POST: /CitiesMvc/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var city = await _db.Cities.FindAsync(id);
            if (city == null) return NotFound();

            _db.Cities.Remove(city);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper: populate ViewData["Countries"] with SelectList of slug->name
        private async Task PopulateCountriesSelectList()
        {
            var countries = await _db.Countries.OrderBy(c => c.Name)
                .Select(c => new { c.Slug, c.Name })
                .ToListAsync();

            ViewData["Countries"] = new SelectList(countries, "Slug", "Name");
        }
    }
}
