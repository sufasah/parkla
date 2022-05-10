using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class DistrictValidator : AbstractValidator<District>
{
    public DistrictValidator()
    {
        CityId();
        Name();
    }
    private void CityId() => RuleFor(x => x.CityId)
        .NotNull();
    private void Name() => RuleFor(x => x.Name)
        .NotNull()
        .NotEmpty()
        .MaximumLength(30);
}