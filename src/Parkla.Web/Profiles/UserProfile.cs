using AutoMapper;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Profiles;
public class UserProfile : Profile
{
    public UserProfile()
    {
        AllowNullDestinationValues = true;
        CreateMap<User, UserDto>();
    }
}