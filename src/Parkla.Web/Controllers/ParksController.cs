using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

    public override async Task<IActionResult> GetAsync(string? id, CancellationToken cancellationToken)
    {
        Guid gid;
        try {
            gid = Guid.Parse(id!);
        }
        catch {
            return BadRequest("Given id must be a valid GUID type and not null");
        }

        var result = await _service.GetAsync(gid, cancellationToken);
        
        if(result == null)
           return NotFound("Entity could not found with given id");
        
        return Ok(result);
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
        var result = await _service.UpdateAsync(
            park, 
            int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value),
            cancellationToken
        ).ConfigureAwait(false);

        return Ok(result);
    }

    public override async Task<IActionResult> DeleteAsync(
        [FromBody] ParkDto dto,
        CancellationToken cancellationToken
    ) {
        var park = _mapper.Map<Park>(dto);          
        await _service.DeleteAsync(
            park, 
            int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value),
            cancellationToken
        ).ConfigureAwait(false);

        return Ok();
    }
}