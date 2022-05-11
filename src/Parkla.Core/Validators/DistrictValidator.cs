using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class DistrictValidator : AbstractValidator<District>
{
    public DistrictValidator()
    {
        Id();
        CityId();
        Name();
     
        RuleSet("id", Id);
    }
    private void Id() => RuleFor(x => x.Id)
        .NotNull();
    private void CityId() => RuleFor(x => x.CityId)
        .NotNull();
    private void Name() => RuleFor(x => x.Name)
        .NotNull()
        .NotEmpty()
        .MaximumLength(30);
}