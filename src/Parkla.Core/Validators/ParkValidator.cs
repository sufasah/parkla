using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class ParkValidator : AbstractValidator<Park>
{
    private readonly float precision30 = (float)Math.Pow(10,30)-1;
    public ParkValidator()
    {
        Id();
        UserId();
        Name();
        Location();
        Latitude();
        Longitude();
        Extras();
        StatusUpdateTime();
        EmptySpace();
        ReservedSpace();
        OccupiedSpace();
        MinPrice();
        AvaragePrice();
        MaxPrice();

        RuleSet("id", Id);
        RuleSet("add", RsAdd);
    }
    private void Id() => RuleFor(x => x.Id)
        .NotNull();
    private void UserId() => RuleFor(x => x.UserId)
        .NotNull();
    private void Name() => RuleFor(x => x.Name)
        .NotNull()
        .NotEmpty()
        .MaximumLength(50);
    private void Location() => RuleFor(x => x.Location)
        .MaximumLength(200)
        .NotNull()
        .NotEmpty();
    private void Latitude() => RuleFor(x => x.Latitude)
        .NotNull()
        .GreaterThanOrEqualTo(-90)
        .LessThanOrEqualTo(90);
    private void Longitude() => RuleFor(x => x.Longitude)
        .NotNull()
        .GreaterThanOrEqualTo(-180)
        .LessThanOrEqualTo(180);
    private void Extras() => RuleFor(x => x.Extras)
        .NotNull()
        .Must(x => x!.Length <= 10);
    private void StatusUpdateTime() => RuleFor(x => x.StatusUpdateTime)
        .NotNull()
        .Must(x => x!.Value.Kind == DateTimeKind.Utc)
        .GreaterThanOrEqualTo(new DateTime(0L, DateTimeKind.Utc))
        .LessThanOrEqualTo(DateTime.UtcNow);
    private void EmptySpace() => RuleFor(x => x.EmptySpace)
        .NotNull();
    private void ReservedSpace() => RuleFor(x => x.ReservedSpace)
        .NotNull();
    private void OccupiedSpace() => RuleFor(x => x.OccupiedSpace)
        .NotNull();
    private void MinPrice() => RuleFor(x => x.MinPrice)
        .NotNull()
        .InclusiveBetween(0,precision30)
        .Must((y, x) => x <= y.AvaragePrice);
    private void AvaragePrice() => RuleFor(x => x.AvaragePrice)
        .NotNull()
        .InclusiveBetween(0,precision30)
        .Must((y, x) => x <= y.MaxPrice);
    private void MaxPrice() => RuleFor(x => x.MaxPrice)
        .NotNull()
        .InclusiveBetween(0,precision30);

    private void RsAdd() {
        Name();
        UserId();
        Location();
        Latitude();
        Longitude();
        Extras();
    }
}