using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class ReservationValidator : AbstractValidator<Reservation>
{
    public ReservationValidator()
    {
        UserId();
        SpaceId();
        PricingId();
        StartTime();
        EndTime();
    }

    private void UserId() => RuleFor(x => x.UserId)
        .NotNull();
    private void SpaceId() => RuleFor(x => x.SpaceId)
        .NotNull();
    private void PricingId() => RuleFor(x => x.PricingId)
        .NotNull();
    private void StartTime() => RuleFor(x => x.StartTime)
        .NotNull()
        .Must(x => x.Kind == DateTimeKind.Utc)
        .GreaterThanOrEqualTo(new DateTime(0L, DateTimeKind.Utc))
        .Must((y, x) => x < y.EndTime);
    private void EndTime() => RuleFor(x => x.EndTime)
        .Must(x => x.Kind == DateTimeKind.Utc)
        .LessThanOrEqualTo(DateTime.UtcNow)
        .NotNull();
}