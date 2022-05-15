using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class ReceivedSpaceStatusValidator : AbstractValidator<ReceivedSpaceStatus>
{
    public ReceivedSpaceStatusValidator()
    {
        Id();
        SpaceId();
        RealSpaceId();
        Status();
        DateTime();
     
        RuleSet("id", Id);
    }
    private void Id() => RuleFor(x => x.Id)
        .NotNull();
    private void SpaceId() => RuleFor(x => x.SpaceId);
    private void RealSpaceId() => RuleFor(x => x.RealSpaceId);
    private void Status() => RuleFor(x => x.Status)
        .NotNull()
        .IsInEnum();
    private void DateTime() => RuleFor(x => x.DateTime)
        .Must(x => x!.Value.Kind == DateTimeKind.Utc)
        .LessThanOrEqualTo(System.DateTime.UtcNow);
}