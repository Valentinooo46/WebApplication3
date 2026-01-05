using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspnetCoreMvcFull.Models.ViewModels.Locations
{
  public class CityDisplayViewModel
  {
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int CountryId { get; init; }
    public string CountryName { get; init; } = string.Empty;
  }

  public class CountryDisplayViewModel
  {
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public IReadOnlyList<CityDisplayViewModel> Cities { get; init; } = new List<CityDisplayViewModel>();
  }

  public class HomeIndexViewModel
  {
    public bool IsAdmin { get; init; }
    public IReadOnlyList<CountryDisplayViewModel> Countries { get; init; } = new List<CountryDisplayViewModel>();
  }

  public class CountryInputModel
  {
    public int? Id { get; set; }

    [Required(ErrorMessage = "Назва країни обов'язкова")]
    [StringLength(200)]
    [Display(Name = "Назва країни")]
    public string Name { get; set; } = string.Empty;
  }

  public class CityInputModel
  {
    public int? Id { get; set; }

    [Required(ErrorMessage = "Назва міста обов'язкова")]
    [StringLength(200)]
    [Display(Name = "Назва міста")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Країна")]
    [Required(ErrorMessage = "Оберіть країну")]
    public int CountryId { get; set; }
  }

  public class AdminLocationsViewModel
  {
    public IReadOnlyList<CountryDisplayViewModel> Countries { get; init; } = new List<CountryDisplayViewModel>();
    public CountryInputModel CountryForm { get; init; } = new();
    public CityInputModel CityForm { get; init; } = new();
    public IEnumerable<SelectListItem> CountryOptions { get; init; } = new List<SelectListItem>();
  }

  public class CityEditorViewModel
  {
    public CityInputModel City { get; init; } = new();
    public IEnumerable<SelectListItem> CountryOptions { get; init; } = new List<SelectListItem>();
  }
}
