using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class PricingService : EntityServiceBase<Pricing>, IPricingService
{
    public PricingService(
        IPricingRepo pricingRepo, 
        IValidator<Pricing> validator
    ) : base(pricingRepo, validator)
    {
    }
}