using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class CityValidator : AbstractValidator<City>
{
    public CityValidator()
    {
        Id();
        Name();

        RuleSet("id", Id);
    }
    private void Id() => RuleFor(x => x.Id)
        .NotNull();
    private void Name() => RuleFor(x => x.Name)
        .NotNull()
        .NotEmpty()
        .MaximumLength(20);
}