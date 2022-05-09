using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class PricingValidator : AbstractValidator<Pricing>
{
    public PricingValidator()
    {
        var precision30 = (float)Math.Pow(10,30)-1;

        RuleFor(x => x.AreaId)
            .NotNull();
        RuleFor(x => x.Unit)
            .NotNull()
            .IsInEnum();
        RuleFor(x => x.Amount)
            .NotNull();
        RuleFor(x => x.Price)
            .NotNull()
            .InclusiveBetween(precision30, 0);
    }
}