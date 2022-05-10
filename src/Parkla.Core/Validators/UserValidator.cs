using System.Linq.Expressions;
using FluentValidation;
using Parkla.Core.Entities;

namespace Parkla.Core.Validators;
public class UserValidator : AbstractValidator<User>
{
    private readonly float precision30 = (float)Math.Pow(10,30)-1;
    public UserValidator()
    {
        Wallet();
        Username();
        Password();
        Email();
        Name();
        Surname();
        Phone();
        Birthdate();
        Gender();
        VerificationCode();
        RefreshTokenSignature();
        CityId();
        DistrictId();
        Address();
        
        RuleSet("register", RsRegister);
    }

    private void Wallet() => RuleFor(x => x.Wallet)
        .NotNull()
        .InclusiveBetween(0,precision30);
    private void Username() => RuleFor(x => x.Username)
        .NotNull()
        .NotEmpty()
        .Matches("[a-zA-Z0-9]+")
        .MaximumLength(30);
    private void Password() => RuleFor(x => x.Password)
        .NotNull()
        .NotEmpty()
        .MinimumLength(6)
        .MaximumLength(20); //hashed so different from database mapping
    private void Email() => RuleFor(x => x.Email)
        .NotNull()
        .NotEmpty()
        .EmailAddress()
        .MaximumLength(320);
    private void Name() => RuleFor(x => x.Name)
        .NotNull()
        .NotEmpty()
        .MaximumLength(50);
    private void Surname() => RuleFor(x => x.Surname)
        .NotNull()
        .NotEmpty()
        .MaximumLength(50);
    private void Phone() => RuleFor(x => x.Phone)
        .NotNull()
        .NotEmpty()
        .MaximumLength(10)
        .Matches("5[0-9]{9}");
    private void Birthdate() => RuleFor(x => x.Birthdate)
        .LessThanOrEqualTo(DateTime.UtcNow.AddYears(-18))
        .GreaterThanOrEqualTo(DateTime.UtcNow.AddYears(-100));
    private void Gender() => RuleFor(x => x.Gender)
        .IsInEnum();
    private void VerificationCode() => RuleFor(x => x.VerificationCode);
    private void RefreshTokenSignature() => RuleFor(x => x.RefreshTokenSignature)
        .MaximumLength(400);
    private void CityId() => RuleFor(x => x.CityId);
    private void DistrictId() => RuleFor(x => x.DistrictId);
    private void Address() => RuleFor(x => x.Address)
        .MaximumLength(200);
    
    private void RsRegister() {
        Username();
        Password();
        Email();
        Name();
        Surname();
        Phone();
        Birthdate();
        Gender();
        CityId();
        DistrictId();
        Address();
    }
}