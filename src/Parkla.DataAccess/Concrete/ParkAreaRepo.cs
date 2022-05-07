using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Context;

namespace Parkla.DataAccess.Concrete;

public class ParkAreaRepo<TContext> : EntityRepoBase<ParkArea, TContext>, IParkAreaRepo 
    where TContext: DbContext, new()
{
    
}