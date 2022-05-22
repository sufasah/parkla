using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class ReceivedSpaceStatusValidator : AbstractValidator<ReceivedSpaceStatus>
{
    public ReceivedSpaceStatusValidator()
    {
        Id();
        SpaceId();
        SpaceName();
        RealSpaceId();
        RealSpaceName();
        OldSpaceStatus();
        NewSpaceStatus();
        OldRealSpaceStatus();
        NewRealSpaceStatus();
        ReceivedTime();
        StatusDataTime();
     
        RuleSet("id", Id);
    }
    private void Id() => RuleFor(x => x.Id)
        .NotNull();
    private void SpaceId() => RuleFor(x => x.SpaceId);
    private void SpaceName() => RuleFor(x => x.SpaceName);
    private void RealSpaceId() => RuleFor(x => x.RealSpaceId);
    private void RealSpaceName() => RuleFor(x => x.RealSpaceName);
    private void OldSpaceStatus() => RuleFor(x => x.OldSpaceStatus)
        .NotNull()
        .IsInEnum();
    private void NewSpaceStatus() => RuleFor(x => x.NewSpaceStatus)
        .NotNull()
        .IsInEnum();
    private void OldRealSpaceStatus() => RuleFor(x => x.OldRealSpaceStatus)
        .NotNull()
        .IsInEnum();
    private void NewRealSpaceStatus() => RuleFor(x => x.NewRealSpaceStatus)
        .NotNull()
        .IsInEnum();
    private void ReceivedTime() => RuleFor(x => x.ReceivedTime)
        .Must(x => x!.Value.Kind == DateTimeKind.Utc)
        .LessThanOrEqualTo(DateTime.UtcNow);
    private void StatusDataTime() => RuleFor(x => x.StatusDataTime)
        .Must(x => x!.Value.Kind == DateTimeKind.Utc)
        .LessThanOrEqualTo(DateTime.UtcNow);
}