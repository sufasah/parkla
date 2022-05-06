using FluentValidation;
using Parkla.Core.DTOs;
using Parkla.Core.Enums;

namespace Collector;
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
            .NotEmpty()
            .IsInEnum()
        ;
        RuleFor(x => x.DateTime)
            .NotNull()
            .NotEmpty()
            .LessThanOrEqualTo(x => DateTime.Now)
        ;
    }
}