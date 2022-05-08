using AutoMapper;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Controllers;

public class ParkAreasController : EntityControllerBase<ParkArea, ParkAreaDto>
{
    private readonly IParkAreaService _service;
    private readonly IMapper _mapper;
    public ParkAreasController(
        IParkAreaService service,
        IMapper mapper
    ) : base(service, mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    
}