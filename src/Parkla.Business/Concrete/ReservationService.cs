using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class ReservationService : EntityServiceBase<Reservation>, IReservationService
{
    public ReservationService(
        IReservationRepo reservationRepo, 
        IValidator<Reservation> validator
    ) : base(reservationRepo, validator)
    {
    }
}