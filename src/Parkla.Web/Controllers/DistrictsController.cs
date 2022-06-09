using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search(
        [FromQuery] int? cityId,
        [FromQuery] string? s,
        CancellationToken cancellationToken
    ) {
        if(string.IsNullOrWhiteSpace(s)) s = "";
        s = s.Trim();

        if(!cityId.HasValue)
            return BadRequest("City id must be given");
        
        var result = await _service.SearchAsync(cityId.Value, s, cancellationToken);
        return Ok(result);
    }
    
}