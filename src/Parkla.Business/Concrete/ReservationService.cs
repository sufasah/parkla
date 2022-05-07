using FluentValidation;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Context;

namespace Parkla.Business.Concrete;
public class ReservationService : EntityServiceBase<Reservation>
{
    public ReservationService(
        IEntityRepository<Reservation> entityRepository, 
        IValidator<Reservation> validator
    ) : base(entityRepository, validator)
    {
    }
}