using AutoMapper;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Controllers;

public class CitiesController : EntityControllerBase<City, CityDto>
{
    private readonly ICityService _service;
    private readonly IMapper _mapper;
    public CitiesController(
        ICityService service,
        IMapper mapper
    ) : base(service, mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    
}