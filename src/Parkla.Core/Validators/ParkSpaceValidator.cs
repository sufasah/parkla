using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class ParkSpaceValidator : AbstractValidator<ParkSpace>
{
    public ParkSpaceValidator()
    {
        Id();
        AreaId();
        RealSpaceId();
        Name();
        StatusUpdateTime();
        Status();
        SpacePath();

        RuleSet("id", Id);
        RuleSet("add", RsAdd);
    }
    private void Id() => RuleFor(x => x.Id)
        .NotNull();
    private void AreaId() => RuleFor(x => x.AreaId)
        .NotNull();
    private void RealSpaceId() => RuleFor(x => x.RealSpaceId);
    private void Name() => RuleFor(x => x.Name)
        .NotNull()
        .NotEmpty()
        .MaximumLength(30);
    private void StatusUpdateTime() => RuleFor(x => x.StatusUpdateTime)
        .Must(x => x!.Value.Kind == DateTimeKind.Utc)
        .LessThanOrEqualTo(DateTime.UtcNow);
    private void Status() => RuleFor(x => x.Status)
        .NotNull()
        .IsInEnum();
    private void SpacePath() => RuleFor(x => x.SpacePath)
        .NotNull()
        .Must(x => x!.Length == 4 && x.Aggregate(true, (prev, y) => y.Length == 2 && prev));
    private void RsAdd()
    {
        AreaId();
        RealSpaceId();
        Name();
        SpacePath();
    }
}