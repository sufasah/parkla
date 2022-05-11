using AutoMapper;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Profiles;
public class RealParkSpaceProfile : Profile
{
    public RealParkSpaceProfile()
    {
        CreateMap<RealParkSpaceDto, RealParkSpace>()
            .ReverseMap();
    }
}