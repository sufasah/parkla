using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Controllers;

public class UsersController : EntityControllerBase<User, UserDto>
{
    private readonly IUserService _service;
    private readonly IMapper _mapper;
    public UsersController(
        IUserService service,
        IMapper mapper
    ) : base(service, mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override async Task<IActionResult> GetAsync(string? id, CancellationToken cancellationToken) 
    {
        var notFound = NotFound("User could not found with given id");
        if(id == null)
            return notFound;
        
        var result = await _service.GetAsync(int.Parse(id), cancellationToken);

        if(result == null)
            return notFound;

        result.Password = null;
        result.RefreshTokenSignature = null;
        result.VerificationCode = null;

        return Ok(result);
    }

    [HttpPut("load-money")]
    public async Task<IActionResult> LoadMoneyAsync(UserDto dto, CancellationToken cancellationToken) {
        if(dto.Id == null || dto.Wallet == null)
            return BadRequest("Id and wallet must be given");
        
        var result = await _service.LoadMoneyAsync(dto.Id.Value, dto.Wallet.Value, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpGet("dashboard/{id}")]
    public async Task<IActionResult> GetDashboardAsync(
        int id, //user id
        CancellationToken cancellationToken
    ) {
        var result = await _service.GetDashboardAsync(id, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }
}