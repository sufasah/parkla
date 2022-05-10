using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class ParkSpaceValidator : AbstractValidator<ParkSpace>
{
    public ParkSpaceValidator()
    {
        RuleFor(x => x.AreaId)
            .NotNull();
        RuleFor(x => x.RealSpaceId);
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .MaximumLength(30);
        RuleFor(x => x.StatusUpdateTime)
            .NotNull()
            .Must(x => x.Kind == DateTimeKind.Utc)
            .GreaterThanOrEqualTo(new DateTime(0L, DateTimeKind.Utc))
            .LessThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.Status)
            .NotNull()
            .IsInEnum();
        RuleFor(x => x.SpacePath)
            .NotNull()
            .Must(x => x.Length == 4 && x.Aggregate(true, (prev, y) => y.Length == 2 && prev));
    }
}