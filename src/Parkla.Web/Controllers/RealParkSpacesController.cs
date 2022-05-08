using AutoMapper;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Controllers;

public class RealParkSpacesController : EntityControllerBase<RealParkSpace, RealParkSpaceDto>
{
    private readonly IRealParkSpaceService _service;
    private readonly IMapper _mapper;
    public RealParkSpacesController(
        IRealParkSpaceService service,
        IMapper mapper
    ) : base(service, mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    
}