using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class RealParkSpaceValidator : AbstractValidator<RealParkSpace>
{
    public RealParkSpaceValidator()
    {
        ParkId();
        SpaceId();
        Name();
        StatusUpdateTime();
        Status();
    }
    private void ParkId() => RuleFor(x => x.ParkId)
        .NotNull();
    private void SpaceId() => RuleFor(x => x.SpaceId);
    private void Name() => RuleFor(x => x.Name)
        .MaximumLength(30)
        .NotEmpty()
        .NotNull();
    private void StatusUpdateTime() => RuleFor(x => x.StatusUpdateTime)
        .NotNull()
        .Must(x => x.Kind == DateTimeKind.Utc)
        .GreaterThanOrEqualTo(new DateTime(0L, DateTimeKind.Utc))
        .LessThanOrEqualTo(DateTime.UtcNow);
    private void Status() => RuleFor(x => x.Status)
        .NotNull()
        .IsInEnum();
}