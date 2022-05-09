using AutoMapper;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Profiles;
public class DistrictProfile : Profile
{
    public DistrictProfile()
    {
        AllowNullDestinationValues = true;
        CreateMap<District, DistrictDto>()
            .ReverseMap();
    }
}