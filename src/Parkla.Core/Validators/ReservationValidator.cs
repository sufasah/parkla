using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class ReservationValidator : AbstractValidator<Reservation>
{
    public ReservationValidator()
    {
        RuleFor(x => x.UserId)
            .NotNull();
        RuleFor(x => x.SpaceId)
            .NotNull();
        RuleFor(x => x.PricingId)
            .NotNull();
        RuleFor(x => x.StartTime)
            .NotNull()
            .Must((y, x) => x < y.EndTime);
        RuleFor(x => x.UserId)
            .NotNull();
    }
}