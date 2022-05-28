using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class ReservationValidator : AbstractValidator<Reservation>
{
    public ReservationValidator()
    {
        Id();
        UserId();
        SpaceId();
        StartTime();
        EndTime();
     
        RuleSet("id", Id);
    }
    private void Id() => RuleFor(x => x.Id)
        .NotNull();
    private void UserId() => RuleFor(x => x.UserId)
        .NotNull();
    private void SpaceId() => RuleFor(x => x.SpaceId)
        .NotNull();
    private void StartTime() => RuleFor(x => x.StartTime)
        .Must(x => x!.Value.Kind == DateTimeKind.Utc)
        .GreaterThanOrEqualTo(DateTime.UtcNow.Subtract(new TimeSpan(0,15,0)))
        .Must((y, x) => x < y.EndTime);
    private void EndTime() => RuleFor(x => x.EndTime)
        .Must(x => x!.Value.Kind == DateTimeKind.Utc)
        .GreaterThan(DateTime.UtcNow.Add(new TimeSpan(0,15,0)))
        .NotNull();
}