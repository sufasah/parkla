using AutoMapper;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Profiles;
public class ReceivedSpaceStatusProfile : Profile
{
    public ReceivedSpaceStatusProfile()
    {
        AllowNullDestinationValues = true;
        CreateMap<ReceivedSpaceStatus, ReceivedSpaceStatusDto>()
            .ReverseMap();
    }
}