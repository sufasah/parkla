using FluentValidation;
using Parkla.Core.DTOs;

namespace Parkla.Core.Validators;
public class ParkSpaceStatusValidator : AbstractValidator<ParkSpaceStatusDto>
{
    public ParkSpaceStatusValidator()
    {
        RuleFor(x => x.ParkId)
            .NotNull()
            .NotEmpty()
        ;
        RuleFor(x => x.SpaceId)
            .NotNull()
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(40)
        ;
        RuleFor(x => x.Status)
            .NotNull()
            .IsInEnum()
        ;
        RuleFor(x => x.DateTime)
            .NotNull()
            .NotEmpty()
            .LessThanOrEqualTo(x => DateTime.UtcNow)
        ;
    }
}