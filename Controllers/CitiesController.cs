using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentValidation.Results;
using WebApplication3.DTOs;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CitiesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IValidator<CityCreateUpdateDto> _validator;

        public CitiesController(AppDbContext db, IMapper mapper, IValidator<CityCreateUpdateDto> validator)
        {
            _db = db;
            _mapper = mapper;
            _validator = validator;
        }

        // GET: api/cities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetAll()
        {
            var cities = await _db.Cities.Include(c => c.Country).OrderBy(c => c.Name).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<CityDto>>(cities));
        }

        // GET: api/cities/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CityDto>> GetById(Guid id)
        {
            var city = await _db.Cities.Include(c => c.Country).FirstOrDefaultAsync(c => c.Id == id);
            if (city == null) return NotFound();
            return Ok(_mapper.Map<CityDto>(city));
        }

        // GET: api/cities/countries  -- список країн (id, name, slug) для фронтенду
        [HttpGet("countries")]
        public async Task<ActionResult<IEnumerable<object>>> GetCountriesList()
        {
            var countries = await _db.Countries
                .OrderBy(c => c.Name)
                .Select(c => new { c.Id, c.Name, c.Slug })
                .ToListAsync();

            return Ok(countries);
        }

        // POST: api/cities
        [HttpPost]
        public async Task<ActionResult<CityDto>> Create([FromBody] CityCreateUpdateDto dto)
        {
            // Не довіряємо Id з клієнта на create
            dto.Id = null;

            ValidationResult result = await _validator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
            }

            // знайдемо країну по slug
            var country = await _db.Countries.SingleOrDefaultAsync(c => c.Slug == dto.CountrySlug);
            if (country == null) return BadRequest(new { message = "Country not found" });

            // double-check uniqueness on DB-level (to produce 409)
            if (await _db.Cities.AnyAsync(c => c.CountryId == country.Id && (c.Name == dto.Name || c.Slug == dto.Slug)))
            {
                return Conflict(new { message = "City name or slug already exists in this country." });
            }

            var entity = _mapper.Map<City>(dto);
            entity.Id = Guid.NewGuid();
            entity.CountryId = country.Id;

            _db.Cities.Add(entity);
            await _db.SaveChangesAsync();

            // load country navigation for DTO
            await _db.Entry(entity).Reference(e => e.Country).LoadAsync();

            var outDto = _mapper.Map<CityDto>(entity);
            return CreatedAtAction(nameof(GetById), new { id = outDto.Id }, outDto);
        }

        // PUT: api/cities/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CityDto>> Update(Guid id, [FromBody] CityCreateUpdateDto dto)
        {
            var existing = await _db.Cities.FindAsync(id);
            if (existing == null) return NotFound();

            // Set Id for validator and mapping
            dto.Id = id;

            ValidationResult result = await _validator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
            }

            // знайдемо країну по slug
            var country = await _db.Countries.SingleOrDefaultAsync(c => c.Slug == dto.CountrySlug);
            if (country == null) return BadRequest(new { message = "Country not found" });

            // Check duplicates excluding current
            if (await _db.Cities.AnyAsync(c => c.Id != id && c.CountryId == country.Id &&
                (c.Name == dto.Name || c.Slug == dto.Slug)))
            {
                return Conflict(new { message = "City name or slug already exists in this country." });
            }

            _mapper.Map(dto, existing);
            existing.CountryId = country.Id;
            existing.Id = id; // ensure remains same

            _db.Cities.Update(existing);
            await _db.SaveChangesAsync();

            await _db.Entry(existing).Reference(e => e.Country).LoadAsync();

            return Ok(_mapper.Map<CityDto>(existing));
        }

        // DELETE: api/cities/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _db.Cities.FindAsync(id);
            if (existing == null) return NotFound();

            _db.Cities.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
