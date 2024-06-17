using AutoMapper;
using TradesWebAPISharedLibrary.DTOs;
using TradesWebAPISharedLibrary.Model;

namespace TradesWebAPI.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EntityDto, Entity>();
            CreateMap<AddressDto, Address>();
            CreateMap<DatesDto, Dates>();
            CreateMap<NameDto, Name>();
        }
    }
}
