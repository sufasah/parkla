using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

    [NonAction]
    public override Task<IActionResult> GetPageAsync(
        [FromQuery] PageDto pageDto,
        [FromQuery] string? s,
        [FromQuery] string? orderBy,
        CancellationToken cancellationToken,
        [FromQuery] bool asc = true
    ) {
        throw new NotImplementedException();
    }

    [HttpGet("")]
    public async Task<IActionResult> GetPageAsync(
        [FromQuery] PageDto pageDto,
        [FromQuery] int? parkId,
        [FromQuery] string? s,
        [FromQuery] string? orderBy,
        CancellationToken cancellationToken,
        [FromQuery] bool asc = true
    ) {
        if(parkId == null)
            return BadRequest("Query parameter 'parkId' must be given");

        var result = await _service.GetPageAsync(parkId.Value, pageDto.NextRecord, pageDto.PageSize, s, orderBy, asc, cancellationToken).ConfigureAwait(false);
        Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Records");
        Response.Headers.Add("X-Total-Records", result.TotalRecords.ToString());
        return Ok(result);
    }
    
}