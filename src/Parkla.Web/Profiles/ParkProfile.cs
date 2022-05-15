using AutoMapper;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Profiles;
public class ParkProfile : Profile
{
    public ParkProfile()
    {
        CreateMap<ParkDto, Park>()
            .ReverseMap();
        
        CreateMap<Park, ParkIncludesUserDto>()
            .ForPath(x => x.User, o => o.MapFrom(x => x.User))
            .ForPath(x => x.User!.Password, o => o.Ignore());
    }
}