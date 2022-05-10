using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class RealParkSpaceValidator : AbstractValidator<RealParkSpace>
{
    public RealParkSpaceValidator()
    {
        RuleFor(x => x.ParkId)
            .NotNull();
        RuleFor(x => x.SpaceId);
        RuleFor(x => x.Name)
            .MaximumLength(30)
            .NotEmpty()
            .NotNull();
        RuleFor(x => x.StatusUpdateTime)
            .NotNull()
            .Must(x => x.Kind == DateTimeKind.Utc)
            .GreaterThanOrEqualTo(new DateTime(0L, DateTimeKind.Utc))
            .LessThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.Status)
            .NotNull()
            .IsInEnum();
    }
}