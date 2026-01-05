using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Models.ViewModels.Locations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AspnetCoreMvcFull.Areas.Admin.Controllers
{
  [Area("Admin")]
  [Authorize(Roles = "Admin")]
  public class LocationsController : Controller
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(ApplicationDbContext dbContext, ILogger<LocationsController> logger)
    {
      _dbContext = dbContext;
      _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
      var model = await BuildAdminViewModel();
      return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCountry([Bind(Prefix = "CountryForm")] CountryInputModel countryForm)
    {
      countryForm.Name = countryForm.Name?.Trim() ?? string.Empty;

      if (!ModelState.IsValid)
      {
        return View("Index", await BuildAdminViewModel(countryForm, new CityInputModel()));
      }

      var exists = await _dbContext.Countries.AnyAsync(c => c.Name.ToLower() == countryForm.Name.ToLower());
      if (exists)
      {
        ModelState.AddModelError("CountryForm.Name", "Країна з такою назвою вже існує.");
        return View("Index", await BuildAdminViewModel(countryForm, new CityInputModel()));
      }

      _dbContext.Countries.Add(new Country { Name = countryForm.Name });
      await _dbContext.SaveChangesAsync();

      TempData["SuccessMessage"] = $"Країну \"{countryForm.Name}\" успішно додано.";
      _logger.LogInformation("Country {Country} created by {User}", countryForm.Name, User.Identity?.Name);

      return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCity([Bind(Prefix = "CityForm")] CityInputModel cityForm)
    {
      cityForm.Name = cityForm.Name?.Trim() ?? string.Empty;

      if (!ModelState.IsValid)
      {
        return View("Index", await BuildAdminViewModel(new CountryInputModel(), cityForm));
      }

      var countryExists = await _dbContext.Countries.AnyAsync(c => c.Id == cityForm.CountryId);
      if (!countryExists)
      {
        ModelState.AddModelError("CityForm.CountryId", "Обрана країна не існує.");
        return View("Index", await BuildAdminViewModel(new CountryInputModel(), cityForm));
      }

      var duplicate = await _dbContext.Cities.AnyAsync(c =>
        c.CountryId == cityForm.CountryId &&
        c.Name.ToLower() == cityForm.Name.ToLower());

      if (duplicate)
      {
        ModelState.AddModelError("CityForm.Name", "Таке місто вже існує в обраній країні.");
        return View("Index", await BuildAdminViewModel(new CountryInputModel(), cityForm));
      }

      _dbContext.Cities.Add(new City
      {
        Name = cityForm.Name,
        CountryId = cityForm.CountryId
      });

      await _dbContext.SaveChangesAsync();

      TempData["SuccessMessage"] = $"Місто \"{cityForm.Name}\" успішно додано.";
      _logger.LogInformation("City {City} created by {User}", cityForm.Name, User.Identity?.Name);

      return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> EditCountry(int id)
    {
      var country = await _dbContext.Countries.FindAsync(id);
      if (country == null)
      {
        return NotFound();
      }

      var model = new CountryInputModel
      {
        Id = country.Id,
        Name = country.Name
      };

      return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCountry(CountryInputModel model)
    {
      model.Name = model.Name?.Trim() ?? string.Empty;

      if (!ModelState.IsValid)
      {
        return View(model);
      }

      var entity = await _dbContext.Countries.FindAsync(model.Id);
      if (entity == null)
      {
        return NotFound();
      }

      var duplicate = await _dbContext.Countries.AnyAsync(c =>
        c.Id != model.Id && c.Name.ToLower() == model.Name.ToLower());

      if (duplicate)
      {
        ModelState.AddModelError(nameof(CountryInputModel.Name), "Країна з такою назвою вже існує.");
        return View(model);
      }

      entity.Name = model.Name;
      await _dbContext.SaveChangesAsync();

      TempData["SuccessMessage"] = $"Країну \"{model.Name}\" оновлено.";
      _logger.LogInformation("Country {Country} edited by {User}", model.Name, User.Identity?.Name);

      return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCountry(int id)
    {
      var entity = await _dbContext.Countries.Include(c => c.Cities).FirstOrDefaultAsync(c => c.Id == id);
      if (entity == null)
      {
        TempData["ErrorMessage"] = "Країну не знайдено.";
        return RedirectToAction(nameof(Index));
      }

      var name = entity.Name;
      _dbContext.Countries.Remove(entity);
      await _dbContext.SaveChangesAsync();

      TempData["SuccessMessage"] = $"Країну \"{name}\" видалено.";
      _logger.LogInformation("Country {Country} deleted by {User}", name, User.Identity?.Name);

      return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> EditCity(int id)
    {
      var city = await _dbContext.Cities.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
      if (city == null)
      {
        return NotFound();
      }

      var model = new CityEditorViewModel
      {
        City = new CityInputModel
        {
          Id = city.Id,
          Name = city.Name,
          CountryId = city.CountryId
        },
        CountryOptions = await GetCountrySelectListAsync(city.CountryId)
      };

      return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCity(CityInputModel model)
    {
      model.Name = model.Name?.Trim() ?? string.Empty;

      if (!ModelState.IsValid)
      {
        return View(new CityEditorViewModel
        {
          City = model,
          CountryOptions = await GetCountrySelectListAsync(model.CountryId)
        });
      }

      var entity = await _dbContext.Cities.FirstOrDefaultAsync(c => c.Id == model.Id);
      if (entity == null)
      {
        return NotFound();
      }

      var duplicate = await _dbContext.Cities.AnyAsync(c =>
        c.Id != model.Id &&
        c.CountryId == model.CountryId &&
        c.Name.ToLower() == model.Name.ToLower());

      if (duplicate)
      {
        ModelState.AddModelError(nameof(CityInputModel.Name), "Таке місто вже існує в обраній країні.");
        return View(new CityEditorViewModel
        {
          City = model,
          CountryOptions = await GetCountrySelectListAsync(model.CountryId)
        });
      }

      entity.Name = model.Name;
      entity.CountryId = model.CountryId;

      await _dbContext.SaveChangesAsync();

      TempData["SuccessMessage"] = $"Місто \"{model.Name}\" оновлено.";
      _logger.LogInformation("City {City} edited by {User}", model.Name, User.Identity?.Name);

      return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCity(int id)
    {
      var entity = await _dbContext.Cities.FirstOrDefaultAsync(c => c.Id == id);
      if (entity == null)
      {
        TempData["ErrorMessage"] = "Місто не знайдено.";
        return RedirectToAction(nameof(Index));
      }

      var name = entity.Name;
      _dbContext.Cities.Remove(entity);
      await _dbContext.SaveChangesAsync();

      TempData["SuccessMessage"] = $"Місто \"{name}\" видалено.";
      _logger.LogInformation("City {City} deleted by {User}", name, User.Identity?.Name);

      return RedirectToAction(nameof(Index));
    }

    private async Task<AdminLocationsViewModel> BuildAdminViewModel(
      CountryInputModel? countryForm = null,
      CityInputModel? cityForm = null)
    {
      var countries = await _dbContext.Countries
        .AsNoTracking()
        .Include(c => c.Cities)
        .OrderBy(c => c.Name)
        .Select(c => new CountryDisplayViewModel
        {
          Id = c.Id,
          Name = c.Name,
          Cities = c.Cities
            .OrderBy(city => city.Name)
            .Select(city => new CityDisplayViewModel
            {
              Id = city.Id,
              Name = city.Name,
              CountryId = c.Id,
              CountryName = c.Name
            })
            .ToList()
        })
        .ToListAsync();

      var options = countries
        .Select(c => new SelectListItem
        {
          Text = c.Name,
          Value = c.Id.ToString(),
          Selected = cityForm?.CountryId == c.Id
        })
        .ToList();

      return new AdminLocationsViewModel
      {
        Countries = countries,
        CountryForm = countryForm ?? new CountryInputModel(),
        CityForm = cityForm ?? new CityInputModel(),
        CountryOptions = options
      };
    }

    private async Task<IEnumerable<SelectListItem>> GetCountrySelectListAsync(int? selectedId = null)
    {
      return await _dbContext.Countries
        .AsNoTracking()
        .OrderBy(c => c.Name)
        .Select(c => new SelectListItem
        {
          Text = c.Name,
          Value = c.Id.ToString(),
          Selected = selectedId == c.Id
        })
        .ToListAsync();
    }
  }
}
