using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Bases;

namespace Parkla.DataAccess.Concrete;

public class PricingRepo<TContext> : EntityRepoBase<Pricing, TContext>, IPricingRepo 
    where TContext: DbContext, new()
{
    
}