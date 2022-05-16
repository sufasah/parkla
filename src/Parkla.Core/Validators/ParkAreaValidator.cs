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
        ReservedSpace();
        OccupiedSpace();
        MinPrice();
        AvaragePrice();
        MaxPrice();
        Pricings();

        RuleSet("id", Id);
        RuleSet("update", RsUpdate);
        RuleSet("templateUpdate", RsTemplateUpdate);
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
    private void ReservedSpace() => RuleFor(x => x.ReservedSpace)
        .NotNull();
    private void OccupiedSpace() => RuleFor(x => x.OccupiedSpace)
        .NotNull();
    private void MinPrice() => RuleFor(x => x.MinPrice)
        .NotNull()
        .InclusiveBetween(0, precision30)
        .Must((y, x) => x <= y.AvaragePrice);
    private void AvaragePrice() => RuleFor(x => x.AvaragePrice)
        .NotNull()
        .InclusiveBetween(0, precision30)
        .Must((y, x) => x <= y.MaxPrice);
    private void MaxPrice() => RuleFor(x => x.MaxPrice)
        .NotNull()
        .InclusiveBetween(0, precision30);

    private void Pricings() => RuleForEach(x => x.Pricings)
        .SetValidator(new PricingValidator(), "add");

    private void Spaces() => RuleForEach(x => x.Spaces)
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
        RuleFor(x => x.TemplateImage)
            .Must(x => {
                if(x==null) return true;
                var i = x.IndexOf('/');
                if(i == -1) return false;
                var i2 = x.IndexOf(',',i+1);
                return i2 != -1;
            }).WithMessage("Template image must be 'mime/type,base64filedata' format");
        Spaces();
    }
}