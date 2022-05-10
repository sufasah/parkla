using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class PricingValidator : AbstractValidator<Pricing>
{
    private readonly float precision30 = (float)Math.Pow(10,30)-1;
    public PricingValidator()
    {
        AreaId();
        Unit();
        Amount();
        Price();
    }

    private void AreaId() => RuleFor(x => x.AreaId)
        .NotNull();
    private void Unit() => RuleFor(x => x.Unit)
        .NotNull()
        .IsInEnum();
    private void Amount() => RuleFor(x => x.Amount)
        .NotNull();
    private void Price() => RuleFor(x => x.Price)
        .NotNull()
        .InclusiveBetween(0, precision30);
}