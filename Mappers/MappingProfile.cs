using AutoMapper;
using WebApplication3.DTOs;
using WebApplication3.Models;

namespace WebApplication3.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<CountryCreateUpdateDto, Country>();
        }
    }
}
