using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Bases;

namespace Parkla.DataAccess.Concrete;

public class DistrictRepo<TContext> : EntityRepoBase<District, TContext>, IDistrictRepo 
    where TContext: DbContext, new()
{
    
}