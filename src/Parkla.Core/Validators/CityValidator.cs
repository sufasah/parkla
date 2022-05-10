using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class CityValidator : AbstractValidator<City>
{
    public CityValidator()
    {
        Name();
    }
    private void Name() => RuleFor(x => x.Name)
        .NotNull()
        .NotEmpty()
        .MaximumLength(20);
}