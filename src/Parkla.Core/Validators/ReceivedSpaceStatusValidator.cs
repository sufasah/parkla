using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class ReceivedSpaceStatusValidator : AbstractValidator<ReceivedSpaceStatus>
{
    public ReceivedSpaceStatusValidator()
    {
        SpaceId();
        RealSpaceId();
        Status();
        DateTime();
    }

    private void SpaceId() => RuleFor(x => x.SpaceId);
    private void RealSpaceId() => RuleFor(x => x.RealSpaceId);
    private void Status() => RuleFor(x => x.Status)
        .NotNull()
        .IsInEnum();
    private void DateTime() => RuleFor(x => x.DateTime)
        .NotNull()
        .Must(x => x.Kind == DateTimeKind.Utc)
        .LessThanOrEqualTo(System.DateTime.UtcNow);
}