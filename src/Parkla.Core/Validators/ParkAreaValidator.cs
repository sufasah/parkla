using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class ParkAreaValidator : AbstractValidator<ParkArea>
{
    public ParkAreaValidator()
    {
        var precision30 = (float)Math.Pow(10,30)-1;

        RuleFor(x => x.ParkId)
            .NotNull();
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(x => x.Description)
            .MaximumLength(200);
        RuleFor(x => x.TemplateImage)
            .NotEmpty()
            .MaximumLength(500);
        RuleFor(x => x.ReservationsEnabled)
            .NotNull();
        RuleFor(x => x.StatusUpdateTime)
            .NotNull()
            .Must(x => x.Kind == DateTimeKind.Utc)
            .GreaterThanOrEqualTo(new DateTime(0L, DateTimeKind.Utc))
            .LessThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.EmptySpace)
            .NotNull();
        RuleFor(x => x.ReservedSpace)
            .NotNull();
        RuleFor(x => x.OccupiedSpace)
            .NotNull();
        RuleFor(x => x.MinPrice)
            .NotNull()
            .InclusiveBetween(0,precision30)
            .Must((y, x) => x <= y.AvaragePrice);
        RuleFor(x => x.AvaragePrice)
            .NotNull()
            .InclusiveBetween(0,precision30)
            .Must((y, x) => x <= y.MaxPrice);
        RuleFor(x => x.MaxPrice)
            .NotNull()
            .InclusiveBetween(0,precision30);
    }
}