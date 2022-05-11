using AutoMapper;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Profiles;
public class ReservationProfile : Profile
{
    public ReservationProfile()
    {
        CreateMap<ReservationDto, Reservation>()
            .ReverseMap();
    }
}