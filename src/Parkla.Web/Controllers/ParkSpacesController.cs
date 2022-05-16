using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

    [NonAction]
    public override Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] int? areaId,
        CancellationToken cancellationToken,
        [FromQuery] bool includeReservations = false
    )
    {
        var result = await _service.GetAllAsync(
            areaId,
            includeReservations,
            cancellationToken
        ).ConfigureAwait(false);
        return Ok(result);
    }

}