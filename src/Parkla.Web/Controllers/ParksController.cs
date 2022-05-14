using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

    public override async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken) {        
        var result = await _service.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return Ok(_mapper.Map<List<ParkIncludesUserDto>>(result));
    }

    public override async Task<IActionResult> UpdateAsync(
        [FromBody] ParkDto dto,
        CancellationToken cancellationToken
    ) {
        var park = _mapper.Map<Park>(dto);          
        await _service.UpdateAsync(
            park, 
            int.Parse(User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value),
            cancellationToken
        ).ConfigureAwait(false);

        return Ok();
    }

    public override async Task<IActionResult> DeleteAsync(
        [FromBody] ParkDto dto,
        CancellationToken cancellationToken
    ) {
        var park = _mapper.Map<Park>(dto);          
        await _service.DeleteAsync(
            park, 
            int.Parse(User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value),
            cancellationToken
        ).ConfigureAwait(false);

        return Ok();
    }
}