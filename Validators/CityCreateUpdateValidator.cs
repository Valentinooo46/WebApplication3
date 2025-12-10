using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApplication3.DTOs;

namespace WebApplication3.Validators
{
    public class CityCreateUpdateValidator : AbstractValidator<CityCreateUpdateDto>
    {
        private readonly AppDbContext _db;

        public CityCreateUpdateValidator(AppDbContext db)
        {
            _db = db;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200)
                .MustAsync(BeUniqueNameInCountry).WithMessage("City name must be unique within the country.");

            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("Slug is required.")
                .MaximumLength(200)
                .MustAsync(BeUniqueSlugInCountry).WithMessage("City slug must be unique within the country.");

            RuleFor(x => x.CountrySlug)
                .NotEmpty().WithMessage("CountrySlug is required.")
                .MustAsync(CountryMustExist).WithMessage("Country with provided slug does not exist.");
        }

        private async Task<bool> CountryMustExist(CityCreateUpdateDto dto, string countrySlug, CancellationToken ct)
        {
            return await _db.Countries.AnyAsync(c => c.Slug == countrySlug, ct);
        }

        private async Task<bool> BeUniqueNameInCountry(CityCreateUpdateDto dto, string name, CancellationToken ct)
        {
            
            var country = await _db.Countries.SingleOrDefaultAsync(c => c.Slug == dto.CountrySlug, ct);
            if (country == null) return false; 

            var id = dto?.Id;
            if (id.HasValue)
            {
                return !await _db.Cities.AnyAsync(c => c.CountryId == country.Id && c.Name == name && c.Id != id.Value, ct);
            }
            else
            {
                return !await _db.Cities.AnyAsync(c => c.CountryId == country.Id && c.Name == name, ct);
            }
        }

        private async Task<bool> BeUniqueSlugInCountry(CityCreateUpdateDto dto, string slug, CancellationToken ct)
        {
            var country = await _db.Countries.SingleOrDefaultAsync(c => c.Slug == dto.CountrySlug, ct);
            if (country == null) return false;

            var id = dto?.Id;
            if (id.HasValue)
            {
                return !await _db.Cities.AnyAsync(c => c.CountryId == country.Id && c.Slug == slug && c.Id != id.Value, ct);
            }
            else
            {
                return !await _db.Cities.AnyAsync(c => c.CountryId == country.Id && c.Slug == slug, ct);
            }
        }
    }
}
