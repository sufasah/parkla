using System.Net;
using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.Core.Exceptions;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class ReservationService : EntityServiceBase<Reservation>, IReservationService
{
    private readonly IReservationRepo _repo;
    private readonly IParklaHubService _parklaHubService;

    private readonly IValidator<Reservation> _validator;

    public ReservationService(
        IReservationRepo reservationRepo, 
        IValidator<Reservation> validator,
        IParklaHubService parklaHubService
    ) : base(reservationRepo, validator)
    {
        _validator = validator;
        _repo = reservationRepo;
        _parklaHubService = parklaHubService;
    }

    public override async Task<Reservation> AddAsync(
        Reservation reservation, 
        CancellationToken cancellationToken = default
    ) {
        var result = await _validator.ValidateAsync(reservation, o => o.IncludeRuleSets("add"), cancellationToken).ConfigureAwait(false);
        if (!result.IsValid)
            throw new ParklaException(result.Errors.First().ToString(), HttpStatusCode.BadRequest);

        var newReservation = await _repo.AddAsync(reservation, cancellationToken);

        if(newReservation.Id != null)
            _ = _parklaHubService.ReservationChangesAsync(reservation, false);

        return newReservation;
    }

    public override async Task DeleteAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        var deletedReservation = await _repo.DeleteReservationAsync(reservation,cancellationToken).ConfigureAwait(false);
        if(deletedReservation != null)
            _ = _parklaHubService.ReservationChangesAsync(deletedReservation, true);
    }

    public async Task<List<Reservation>> GetUserReservationsAsync(
        int userId,
        CancellationToken cancellationToken
    ) {
        return await _repo.UserReservationListAsync(userId, cancellationToken).ConfigureAwait(false);
    }
}