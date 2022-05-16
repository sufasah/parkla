using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

    [NonAction]
    public override Task<IActionResult> UpdateAsync(ParkAreaDto dto, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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