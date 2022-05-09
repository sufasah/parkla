using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class ReceivedSpaceStatusValidator : AbstractValidator<ReceivedSpaceStatus>
{
    public ReceivedSpaceStatusValidator()
    {
        RuleFor(x => x.SpaceId);
        RuleFor(x => x.RealSpaceId);
        RuleFor(x => x.Status)
            .NotNull()
            .IsInEnum();
        RuleFor(x => x.DateTime)
            .NotNull()
            .GreaterThanOrEqualTo(new DateTime(0L, DateTimeKind.Utc))
            .LessThanOrEqualTo(DateTime.UtcNow);
    }
}