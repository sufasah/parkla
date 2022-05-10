using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        var precision30 = (float)Math.Pow(10,30)-1;

        RuleFor(x => x.Wallet)
            .NotNull()
            .InclusiveBetween(0,precision30);
        RuleFor(x => x.Username)
            .NotNull()
            .NotEmpty()
            .MaximumLength(30);
        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty()
            .MaximumLength(20); //hashed to 50 so different from database mapping
        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(x => x.Surname)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(x => x.Phone)
            .NotNull()
            .NotEmpty()
            .MaximumLength(20);
        RuleFor(x => x.Birthdate);
        RuleFor(x => x.Gender)
            .IsInEnum();
        RuleFor(x => x.VerificationCode);
        RuleFor(x => x.RefreshTokenSignature)
            .MaximumLength(400);
        RuleFor(x => x.CityId);
        RuleFor(x => x.DistrictId);
        RuleFor(x => x.Address)
            .MaximumLength(200)
            .NotEmpty();
        
    }
}