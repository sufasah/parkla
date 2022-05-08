using AutoMapper;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Controllers;

public class ReservationsController : EntityControllerBase<Reservation, ReservationDto>
{
    private readonly IReservationService _service;
    private readonly IMapper _mapper;
    public ReservationsController(
        IReservationService service,
        IMapper mapper
    ) : base(service, mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    
}