using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Controllers
{
    using AutoMapper;
    using FluentValidation;
    using FluentValidation.Results;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using WebApplication3.DTOs;
    using WebApplication3.Models;

    namespace Server.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        [Produces("application/json")]
        public class CountriesController : ControllerBase
        {
            private readonly AppDbContext _db;
            private readonly IMapper _mapper;
            private readonly IValidator<CountryCreateUpdateDto> _validator;

            public CountriesController(AppDbContext db, IMapper mapper, IValidator<CountryCreateUpdateDto> validator)
            {
                _db = db;
                _mapper = mapper;
                _validator = validator;
            }

            // GET: api/countries
            [HttpGet]
            public async Task<ActionResult<IEnumerable<object>>> GetAll()
            {
                var countries = await _db.Countries.OrderBy(c => c.Name).ToListAsync();
                return Ok(_mapper.Map<IEnumerable<object>>(countries));
            }

            // GET api/countries/{id}
            [HttpGet("{id:guid}")]
            public async Task<ActionResult<object>> GetById(Guid id)
            {
                var c = await _db.Countries.FindAsync(id);
                if (c == null) return NotFound();
                return Ok(_mapper.Map<object>(c));
            }

            // POST api/countries
            [HttpPost]
            public async Task<ActionResult<object>> Create([FromBody] CountryCreateUpdateDto dto)
            {
                // Ensure incoming dto doesn't carry an Id from client (ignore if present)
                dto.Id = null;

                ValidationResult result = await _validator.ValidateAsync(dto);
                if (!result.IsValid)
                {
                    return BadRequest(result.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
                }

                if (await _db.Countries.AnyAsync(x => x.Name == dto.Name || x.Slug == dto.Slug || x.Code == dto.Code))
                {
                    return Conflict(new { message = "Name, Slug or Code already exists." });
                }

                var entity = _mapper.Map<Country>(dto);
                entity.Id = Guid.NewGuid();
                _db.Countries.Add(entity);
                await _db.SaveChangesAsync();

                var outDto = _mapper.Map<object>(entity);
                return CreatedAtAction(nameof(GetById), new { id = entity.Id }, outDto);
            }

            // PUT api/countries/{id}
            [HttpPut("{id:guid}")]
            public async Task<ActionResult<object>> Update(Guid id, [FromBody] CountryCreateUpdateDto dto)
            {
                var existing = await _db.Countries.FindAsync(id);
                if (existing == null) return NotFound();

                // Set route id into dto.Id for validator and mapping
                dto.Id = id;

                ValidationResult result = await _validator.ValidateAsync(dto);
                if (!result.IsValid)
                {
                    return BadRequest(result.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
                }

                if (await _db.Countries.AnyAsync(x => x.Id != id && (x.Name == dto.Name || x.Slug == dto.Slug || x.Code == dto.Code)))
                {
                    return Conflict(new { message = "Name, Slug or Code already exists." });
                }

                _mapper.Map(dto, existing);
                // ensure Id stays the same
                existing.Id = id;

                _db.Countries.Update(existing);
                await _db.SaveChangesAsync();

                return Ok(_mapper.Map<object>(existing));
            }

            // DELETE api/countries/{id}
            [HttpDelete("{id:guid}")]
            public async Task<IActionResult> Delete(Guid id)
            {
                var existing = await _db.Countries.FindAsync(id);
                if (existing == null) return NotFound();

                _db.Countries.Remove(existing);
                await _db.SaveChangesAsync();
                return NoContent();
            }
        }
    }
}
