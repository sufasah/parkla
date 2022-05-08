using AutoMapper;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Profiles;
public class ParkProfile : Profile
{
    public ParkProfile()
    {
        AllowNullDestinationValues = true;
        CreateMap<Park, ParkDto>();
    }
}