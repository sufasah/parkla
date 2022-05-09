using AutoMapper;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Profiles;
public class ParkSpaceProfile : Profile
{
    public ParkSpaceProfile()
    {
        AllowNullDestinationValues = true;
        CreateMap<ParkSpace, ParkSpaceDto>()
            .ReverseMap();
    }
}