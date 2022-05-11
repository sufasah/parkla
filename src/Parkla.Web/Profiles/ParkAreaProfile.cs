using AutoMapper;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Profiles;
public class ParkAreaProfile : Profile
{
    public ParkAreaProfile()
    {
        CreateMap<ParkAreaDto, ParkArea>()
            .ReverseMap();
    }
}