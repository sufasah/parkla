using AutoMapper;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Profiles;
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<UserDto, User>()
            .ForMember(
                x => x.Birthdate,
                o => o.MapFrom(x => x.Birthdate.HasValue 
                    ? DateTime.SpecifyKind(x.Birthdate.Value, DateTimeKind.Utc)
                    : x.Birthdate));
    }
}