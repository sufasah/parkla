using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<List<City>> Search(
        [FromQuery] string s,
        CancellationToken cancellationToken
    ) {
        if(string.IsNullOrWhiteSpace(s)) s = "";
        s = s.Trim();
        return await _service.SearchAsync(s, cancellationToken);
    }
}