using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class ParkValidator : AbstractValidator<Park>
{
    public ParkValidator()
    {
        var precision30 = (float)Math.Pow(10,30)-1;

        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(x => x.Location)
            .MaximumLength(200)
            .NotNull()
            .NotEmpty();
        RuleFor(x => x.Latitude)
            .NotNull()
            .GreaterThanOrEqualTo(-90)
            .LessThanOrEqualTo(90);
        RuleFor(x => x.Longitude)
            .NotNull()
            .GreaterThanOrEqualTo(-180)
            .LessThanOrEqualTo(180);
        RuleFor(x => x.Extras)
            .NotNull()
            .Must(x => x.Length <= 10);
        RuleFor(x => x.StatusUpdateTime)
            .NotNull()
            .GreaterThanOrEqualTo(new DateTime(0L, DateTimeKind.Utc))
            .LessThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.ReservedSpace)
            .NotNull();
        RuleFor(x => x.OccupiedSpace)
            .NotNull();
        RuleFor(x => x.MinPrice)
            .NotNull()
            .InclusiveBetween(precision30,0)
            .Must((y, x) => x <= y.AvaragePrice);
        RuleFor(x => x.AvaragePrice)
            .NotNull()
            .InclusiveBetween(precision30,0)
            .Must((y, x) => x <= y.MaxPrice);
        RuleFor(x => x.MaxPrice)
            .NotNull()
            .InclusiveBetween(precision30,0);
    }
}