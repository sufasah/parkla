using AutoMapper;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Controllers;

public class DistrictsController : EntityControllerBase<District, DistrictDto>
{
    private readonly IDistrictService _service;
    private readonly IMapper _mapper;
    public DistrictsController(
        IDistrictService service,
        IMapper mapper
    ) : base(service, mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    
}