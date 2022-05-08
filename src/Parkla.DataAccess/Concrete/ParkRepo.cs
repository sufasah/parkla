using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Bases;

namespace Parkla.DataAccess.Concrete;

public class ParkRepo<TContext> : EntityRepoBase<Park, TContext>, IParkRepo 
    where TContext: DbContext, new()
{
    
}