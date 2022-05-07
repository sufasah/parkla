using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Bases;

namespace Parkla.DataAccess.Concrete;

public class ParkSpaceRepo<TContext> : EntityRepoBase<ParkSpace, TContext>, IParkSpaceRepo 
    where TContext: DbContext, new()
{
    
}