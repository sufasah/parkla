using AutoMapper;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Profiles;
public class CityProfile : Profile
{
    public CityProfile()
    {
        AllowNullDestinationValues = true;
        CreateMap<City, CityDto>();
    }
}