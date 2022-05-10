using AutoMapper;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Profiles;
public class ParkProfile : Profile
{
    public ParkProfile()
    {
        CreateMap<Park, ParkDto>()
            .ReverseMap();
    }
}