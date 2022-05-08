using AutoMapper;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Profiles;
public class PricingProfile : Profile
{
    public PricingProfile()
    {
        AllowNullDestinationValues = true;
        CreateMap<Pricing, PricingDto>();
    }
}