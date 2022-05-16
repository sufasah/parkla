using AutoMapper;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Controllers;

public class PricingsController : EntityControllerBase<Pricing, PricingDto>
{
    private readonly IPricingService _service;
    private readonly IMapper _mapper;
    public PricingsController(
        IPricingService service,
        IMapper mapper
    ) : base(service, mapper)
    {
        _service = service;
        _mapper = mapper;
    }


}