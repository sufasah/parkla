using System.Linq.Expressions;
using System.Text.RegularExpressions;
using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class ParkAreaValidator : AbstractValidator<ParkArea>
{
    private readonly float precision30 = (float)Math.Pow(10, 30) - 1;
    public ParkAreaValidator()
    {
        Id();
        ParkId();
        Name();
        Description();
        TemplateImage();
        ReservationsEnabled();
        StatusUpdateTime();
        EmptySpace();
        OccupiedSpace();
        MinPrice();
        AveragePrice();
        MaxPrice();
        Pricings();

        RuleSet("id", Id);
        RuleSet("update", RsUpdate);
        RuleSet("templateUpdate", RsTemplateUpdate);
        RuleSet("template", RsTemplate);
    }
    private void Id() => RuleFor(x => x.Id)
        .NotNull();
    private void ParkId() => RuleFor(x => x.ParkId)
        .NotNull();
    private void Name() => RuleFor(x => x.Name)
        .NotNull()
        .NotEmpty()
        .MaximumLength(50);
    private void Description() => RuleFor(x => x.Description)
        .MaximumLength(200);
    private void TemplateImage() => RuleFor(x => x.TemplateImage)
        .NotEmpty()
        .Null()
        .MaximumLength(500);
    private void ReservationsEnabled() => RuleFor(x => x.ReservationsEnabled)
        .NotNull();
    private void StatusUpdateTime() => RuleFor(x => x.StatusUpdateTime)
        .Must(x => x!.Value.Kind == DateTimeKind.Utc)
        .LessThanOrEqualTo(DateTime.UtcNow);
    private void EmptySpace() => RuleFor(x => x.EmptySpace)
        .NotNull();
    private void OccupiedSpace() => RuleFor(x => x.OccupiedSpace)
        .NotNull();
    private void MinPrice() => RuleFor(x => x.MinPrice)
        .NotNull()
        .InclusiveBetween(0, precision30)
        .Must((y, x) => x <= y.AveragePrice);
    private void AveragePrice() => RuleFor(x => x.AveragePrice)
        .NotNull()
        .InclusiveBetween(0, precision30)
        .Must((y, x) => x <= y.MaxPrice);
    private void MaxPrice() => RuleFor(x => x.MaxPrice)
        .NotNull()
        .InclusiveBetween(0, precision30);

    private void Pricings() => RuleForEach(x => x.Pricings)
        .NotNull()
        .SetValidator(new PricingValidator(), "add");

    private void Spaces() => RuleForEach(x => x.Spaces)
        .NotNull()
        .Must(x => x.RealSpace == null).WithMessage("RealSpace field of spaces must be null")
        .Must(x => x.Reservations == null || !x.Reservations.Any()).WithMessage("Reservations field of spaces must be null or empty")
        .Must(x => x.ReceivedSpaceStatusses == null || !x.ReceivedSpaceStatusses.Any()).WithMessage("ReceivedSpaceStasusses field of spaces must be null or empty")
        .SetValidator(new ParkSpaceValidator(), "add");

    private void RsUpdate()
    {
        Name();
        Description();
        ReservationsEnabled();
        Pricings();
    }

    private void RsTemplateUpdate()
    {
        Spaces();
        RuleFor(x => x.Spaces)
            .Must(x => {
                return x.DistinctBy(x => x.RealSpaceId).Count() == x.Count;
            }).WithMessage("ParkSpaces must have distinct RealSpace id values. One RealSpace can only have one ParkSpace.");
    }

    private void RsTemplate() {
        RuleFor(x => x.TemplateImage)
            .Must(x => {
                if(x==null) return true;
                var i = x.IndexOf('/');
                if(i == -1) return false;
                var i2 = x.IndexOf(',',i+1);
                return i2 != -1;
            });
    }
}