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

    
}