using AutoMapper;
using WebApplication3.DTOs;
using WebApplication3.Models;

namespace WebApplication3.Mappers
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            
            CreateMap<City, CityDto>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId))
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country != null ? src.Country.Name : string.Empty))
                .ForMember(dest => dest.CountrySlug, opt => opt.MapFrom(src => src.Country != null ? src.Country.Slug : string.Empty));

            
            CreateMap<City, CityCreateUpdateDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CountrySlug, opt => opt.MapFrom(src => src.Country != null ? src.Country.Slug : string.Empty))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            
            CreateMap<CityCreateUpdateDto, City>()
                
                .ForMember(dest => dest.Id, opt =>
                {
                    opt.PreCondition(src => src.Id.HasValue);
                    opt.MapFrom(src => src.Id!.Value);
                })
                .ForMember(dest => dest.Country, opt => opt.Ignore())
                
                .ForMember(dest => dest.CountryId, opt => opt.Ignore());
        }
    }
}
