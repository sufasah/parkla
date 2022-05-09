using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class DistrictValidator : AbstractValidator<District>
{
    public DistrictValidator()
    {
        RuleFor(x => x.CityId)
            .NotNull();
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .MaximumLength(30);
    }
}