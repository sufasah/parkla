using AutoMapper;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Controllers;

public class ParkSpacesController : EntityControllerBase<ParkSpace, ParkSpaceDto>
{
    private readonly IParkSpaceService _service;
    private readonly IMapper _mapper;
    public ParkSpacesController(
        IParkSpaceService service,
        IMapper mapper
    ) : base(service, mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    
}