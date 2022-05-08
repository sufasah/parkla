using AutoMapper;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Controllers;

public class ParksController : EntityControllerBase<Park, ParkDto>
{
    private readonly IParkService _service;
    private readonly IMapper _mapper;
    public ParksController(
        IParkService service,
        IMapper mapper
    ) : base(service, mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    
}