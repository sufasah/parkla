using FluentValidation;
using Parkla.Core.DTOs;
using Parkla.Core.Enums;

namespace Collector;
public class ParkSpaceStatusValidator : AbstractValidator<ParkSpaceStatusDto>
{
    public ParkSpaceStatusValidator()
    {
        RuleFor(x => x.Parkid)
            .NotNull()
            .NotEmpty()
        ;
        RuleFor(x => x.Spaceid)
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