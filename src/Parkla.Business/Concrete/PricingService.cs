using FluentValidation;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Context;

namespace Parkla.Business.Concrete;
public class PricingService : EntityServiceBase<Pricing>
{
    public PricingService(
        IEntityRepository<Pricing> entityRepository, 
        IValidator<Pricing> validator
    ) : base(entityRepository, validator)
    {
    }
}