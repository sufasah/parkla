using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Bases;

namespace Parkla.DataAccess.Concrete;

public class RealParkSpaceRepo<TContext> : EntityRepoBase<RealParkSpace, TContext>, IRealParkSpaceRepo 
    where TContext: DbContext, new()
{
    
}