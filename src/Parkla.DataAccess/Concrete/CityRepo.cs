using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Context;

namespace Parkla.DataAccess.Concrete;

public class CityRepo<TContext> : EntityRepoBase<City, TContext>, ICityRepo 
    where TContext: DbContext, new()
{
    
}