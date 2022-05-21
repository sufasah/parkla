using AutoMapper;
using Parkla.Core.Entities;
using Parkla.Core.Helpers;
using Parkla.Core.Models;
using Parkla.Web.Models;

namespace Parkla.Web.Profiles;
public class ParkAreaProfile : Profile
{
    public ParkAreaProfile()
    {
        CreateMap<ParkAreaDto, ParkArea>()
            .ReverseMap();
        
        CreateMap<InstantParkAreaReservedSpace, ParkAreaDto>()
            .IncludeMembers(x => x.ParkArea)
            .ForMember(x => x.ReservedSpace, o => o.MapFrom(y => y.ReservedSpaceCount));
        
        CreateMap<ParkArea, ParkArea>()
            .ReverseMap();
        
    }
}