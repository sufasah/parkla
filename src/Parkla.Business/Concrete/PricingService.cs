using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class PricingService : EntityServiceBase<Pricing>, IPricingService
{
    private readonly IPricingRepo _pricingRepo;
    private readonly IValidator<Pricing> _validator;

    public PricingService(
        IPricingRepo pricingRepo, 
        IValidator<Pricing> validator
    ) : base(pricingRepo, validator)
    {
        _pricingRepo = pricingRepo;
        _validator = validator;
    }
}