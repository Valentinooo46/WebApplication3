using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Models.ViewModels.Locations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Xml.Linq;

namespace AspnetCoreMvcFull.Controllers;

public class HomeController : Controller
{
  public IActionResult Privacy()
  {
    return View();
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
  {
    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
  }
  private readonly ILogger<HomeController> _logger;
  private readonly ApplicationDbContext _dbContext;

  public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
  {
    _logger = logger;
    _dbContext = dbContext;
  }

  public async Task<IActionResult> Index()
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

    var model = new HomeIndexViewModel
    {
      IsAdmin = User.IsInRole("Admin"),
      Countries = countries
    }
    ;

    return View(model);
  }
}
