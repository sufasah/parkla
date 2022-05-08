using AutoMapper;
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

    
}