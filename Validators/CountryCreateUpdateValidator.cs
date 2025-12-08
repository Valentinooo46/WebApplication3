using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApplication3.DTOs;

namespace WebApplication3.Validators
{
    public class CountryCreateUpdateValidator : AbstractValidator<CountryCreateUpdateDto>
    {
        private readonly AppDbContext _db;

        public CountryCreateUpdateValidator(AppDbContext db)
        {
            _db = db;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200)
                .MustAsync(BeUniqueName).WithMessage("Name must be unique.");

            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("Slug is required.")
                .MaximumLength(200)
                .MustAsync(BeUniqueSlug).WithMessage("Slug must be unique.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(10)
                .MustAsync(BeUniqueCode).WithMessage("Code must be unique.");

            RuleFor(x => x.PhoneCode)
                .NotEmpty().WithMessage("PhoneCode is required.")
                .MaximumLength(10);
        }

        private async Task<bool> BeUniqueName(CountryCreateUpdateDto dto, string name, CancellationToken ct)
        {
            var id = dto?.Id;
            if (id.HasValue)
            {
                return !await _db.Countries.AnyAsync(c => c.Name == name && c.Id != id.Value, ct);
            }
            else
            {
                return !await _db.Countries.AnyAsync(c => c.Name == name, ct);
            }
        }

        private async Task<bool> BeUniqueSlug(CountryCreateUpdateDto dto, string slug, CancellationToken ct)
        {
            var id = dto?.Id;
            if (id.HasValue)
            {
                return !await _db.Countries.AnyAsync(c => c.Slug == slug && c.Id != id.Value, ct);
            }
            else
            {
                return !await _db.Countries.AnyAsync(c => c.Slug == slug, ct);
            }
        }

        private async Task<bool> BeUniqueCode(CountryCreateUpdateDto dto, string code, CancellationToken ct)
        {
            var id = dto?.Id;
            if (id.HasValue)
            {
                return !await _db.Countries.AnyAsync(c => c.Code == code && c.Id != id.Value, ct);
            }
            else
            {
                return !await _db.Countries.AnyAsync(c => c.Code == code, ct);
            }
        }
    }
}
