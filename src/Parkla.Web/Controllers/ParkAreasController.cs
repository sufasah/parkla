using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Core.Helpers;
using Parkla.Core.Models;
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

    [NonAction]
    public override Task<IActionResult> UpdateAsync(ParkAreaDto dto, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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
        [FromQuery] Guid? parkId,
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
        return Ok(_mapper.Map<List<InstantParkAreaReservedSpace>,List<ParkAreaDto>>(result));
    }

    [HttpPut("")]
    public async Task<IActionResult> UpdateAsync(
        [FromBody] ParkAreaDto dto,
        CancellationToken cancellationToken,
        [FromQuery] bool templateMode = false
    )
    {
        var parkArea = _mapper.Map<ParkArea>(dto);
        var result = await _service.UpdateAsync(
            parkArea,
            int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value),
            templateMode,
            cancellationToken
        ).ConfigureAwait(false);

        return Ok(result);
    }

    public override async Task<IActionResult> DeleteAsync(
        [FromBody] ParkAreaDto dto,
        CancellationToken cancellationToken
    )
    {
        var parkArea = _mapper.Map<ParkArea>(dto);
        await _service.DeleteAsync(
            parkArea,
            int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value),
            cancellationToken
        ).ConfigureAwait(false);

        return Ok();
    }

}