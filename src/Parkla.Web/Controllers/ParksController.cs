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
        return Ok(_mapper.Map<List<ParkAllDto>>(result));
    }

    public override async Task<IActionResult> UpdateAsync(
        [FromBody] ParkDto dto,
        CancellationToken cancellationToken
    ) {
        var badRequest = BadRequest($"User requested is not permitted to update or delete other user's park.");
        
        if(!IsUserPermitted(dto))
            return badRequest;
            
        return await base.UpdateAsync(dto,cancellationToken).ConfigureAwait(false);
    }

    public override async Task<IActionResult> DeleteAsync(
        [FromBody] ParkDto dto,
        CancellationToken cancellationToken
    ) {
        var badRequest = BadRequest($"User requested is not permitted to update or delete other user's park.");
        
        if(!IsUserPermitted(dto))
            return badRequest;
            
        return await base.DeleteAsync(dto,cancellationToken).ConfigureAwait(false);
    }

    private bool IsUserPermitted(ParkDto dto) {
        if(dto.UserId == null)
            return false;
        
        if(User.HasClaim(JwtRegisteredClaimNames.Sub, dto.UserId.ToString()!))
            return false;

        return true;
    }
    
}