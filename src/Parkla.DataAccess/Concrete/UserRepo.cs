using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Context;

namespace Parkla.DataAccess.Concrete;

public class UserRepo<TContext> : EntityRepoBase<Reservation, TContext>, IReservationRepo 
    where TContext: DbContext, new()
{
    
}