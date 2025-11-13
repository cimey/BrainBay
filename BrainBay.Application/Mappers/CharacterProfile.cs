using AutoMapper;
using BrainBay.Core.Entities;
using BrainBay.Core.ValueTypes;
using BrainBay.Model.Responses;

namespace BrainBay.Application.Mappers
{
    public class CharacterProfile : Profile
    {
        public CharacterProfile()
        {
            CreateMap<Character, CharacterDto>()
                .ForMember(dest => dest.Episode, opt => opt.MapFrom(src =>
                    src.Episodes.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()))
                .ForMember(dest => dest.Origin, opt => opt.MapFrom(src =>
                    new OriginResponse(src.Origin.Name, src.Origin.Url)))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src =>
                    new LocationResponse(src.Location.Name, src.Location.Url)));

            CreateMap<CharacterDto, Character>()
                .ForMember(dest => dest.Episodes, opt => opt.MapFrom(src => string.Join(",", src.Episode)))
                .ForMember(dest => dest.Origin, opt => opt.MapFrom(src =>
                    new Origin(src.Origin.Name, src.Origin.Url)))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src =>
                    new Location(src.Location.Name, src.Location.Url)));
        }
    }
}
