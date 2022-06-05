using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Core.Helpers;
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

    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetUserReservationsAsync(
        int id, // userId
        CancellationToken cancellationToken
    ) {
        var result = await _service.GetUserReservationsAsync(id, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }
}