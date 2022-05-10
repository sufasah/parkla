using FluentValidation;
using Parkla.Core.DTOs;

namespace Parkla.Core.Validators;
public class ParkSpaceStatusValidator : AbstractValidator<ParkSpaceStatusDto>
{
    public ParkSpaceStatusValidator()
    {
        ParkId();
        SpaceId();
        Status();
        DateTime();
    }
    private void ParkId() => RuleFor(x => x.ParkId)
        .NotNull()
        .NotEmpty();
    private void SpaceId() => RuleFor(x => x.SpaceId)
        .NotNull()
        .NotEmpty()
        .MinimumLength(1)
        .MaximumLength(40);
    private void Status() => RuleFor(x => x.Status)
        .NotNull()
        .IsInEnum();
    private void DateTime() => RuleFor(x => x.DateTime)
        .NotNull()
        .NotEmpty()
        .Must(x => x.Kind == DateTimeKind.Utc)
        .LessThanOrEqualTo(x => System.DateTime.UtcNow);
}