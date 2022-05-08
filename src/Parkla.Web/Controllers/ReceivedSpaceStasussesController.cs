using AutoMapper;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Controllers;

public class ReceivedSpaceStasussesController : EntityControllerBase<ReceivedSpaceStatus, ReceivedSpaceStatusDto>
{
    private readonly IReceivedSpaceStatusService _service;
    private readonly IMapper _mapper;
    public ReceivedSpaceStasussesController(
        IReceivedSpaceStatusService service,
        IMapper mapper
    ) : base(service, mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    
}