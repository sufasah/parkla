using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using FluentValidation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Parkla.Business.Abstract;
using Parkla.Core.DTOs;
using Parkla.Core.Entities;
using Parkla.Core.Exceptions;
using Parkla.Core.Helpers;
using Parkla.Core.Options;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class AuthService : IAuthService
{
    private readonly IMailService _mailService;
    private readonly IValidator<User> _validatior;
    private readonly IUserRepo _userRepo;
    private readonly SecretOptions _secretOptions;

    public AuthService(
        IMailService mailService,
        IValidator<User> validator,
        IUserRepo userRepo,
        IOptions<SecretOptions> secretOptions
    ) {
        _mailService = mailService;
        _validatior = validator;
        _userRepo = userRepo;
        _secretOptions = secretOptions.Value;
    }

    public async Task RevokeRefreshToken(string oldRefreshToken) {
        var user = await ValidateRefreshToken(oldRefreshToken).ConfigureAwait(false);
        user.RefreshTokenSignature = null;
        await _userRepo.UpdateAsync(user);
    }
    public async Task<TokensDto> RefreshToken(string oldRefreshToken) {
        var user = await ValidateRefreshToken(oldRefreshToken).ConfigureAwait(false);
        var tokens = JwtHelper.GetTokens(user);
        user.RefreshTokenSignature = JwtHelper.TokenHandler.ReadJwtToken(tokens.RefreshToken).RawSignature;
        await _userRepo.UpdateAsync(user).ConfigureAwait(false);
        return tokens;
    }

    private async Task<User> ValidateRefreshToken(string oldRefreshToken) {
        TokenValidationResult validationResult;
        var tokenNotValid = new ParklaException("Refresh token is not a valid jwt token", HttpStatusCode.BadRequest);
        
        try {
            validationResult = await JwtHelper.TokenHandler.ValidateTokenAsync(oldRefreshToken, JwtHelper.TokenValidationParameters).ConfigureAwait(false);
        } catch {
            throw tokenNotValid;
        }
        
        if(!validationResult.IsValid) throw tokenNotValid;
        
        int id;
        JwtSecurityToken token;
        try {
            token = (validationResult.SecurityToken as JwtSecurityToken)!;
            var claim = token!.Payload.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            id = int.Parse(claim!.Value);
        } catch {
            throw tokenNotValid;
        }

        var signature = token.RawSignature;

        var user = await _userRepo.GetAsync(x => x.Id == id).ConfigureAwait(false);

        var userTokenNotMatch =  new ParklaException("Refresh token and its user does not match. User has this token is not found", HttpStatusCode.NotFound);

        if(user == null) 
            throw userTokenNotMatch;

        if(user.RefreshTokenSignature != signature)
            throw userTokenNotMatch;
        
        return user;
    }

    public async Task RegisterAsync(User user, CancellationToken cancellationToken = default) {
        var result = _validatior.Validate(user);
        if (!result.IsValid)
            throw new ParklaException(result.ToString(), HttpStatusCode.BadRequest);

        var dbUser = await _userRepo.GetAsync(x => x.Username == user.Username, cancellationToken).ConfigureAwait(false);
        if (dbUser != null)
            throw new ParklaException("Already a user exists with given username. Every user must have unique username", HttpStatusCode.Forbidden);

        user.VerificationCode = Guid.NewGuid().ToString().Split('-').First().ToUpper();
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, BCrypt.Net.BCrypt.GenerateSalt(9) + _secretOptions.PasswordPepper);
        user = await _userRepo.AddAsync(user, cancellationToken).ConfigureAwait(false);
        
        await SendEmailAsync(user, "E-Mail Verification | Parkla", $"Dear {user.Name}, Verification of account is necessary to registeration. You need to verify your account with the code below:\nVerification Code: {user.VerificationCode}").ConfigureAwait(false);
    }

    public async Task<TokensDto> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var notValid = new ParklaException("Username or password is not valid", HttpStatusCode.BadRequest);
        if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw notValid;
        if(username.Length > 30 || password.Length > 50)
            throw notValid;

        var user = await _userRepo.GetAsync(x => x.Username == username, cancellationToken).ConfigureAwait(false);
        var notFound = new ParklaException("User with given username and password is not found", HttpStatusCode.NotFound);
        if (user == null) throw notFound;

        if (user.VerificationCode != null) {
            user.VerificationCode = Guid.NewGuid().ToString().Split('-').First().ToUpper();
            user = await _userRepo.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
            await SendEmailAsync(user, "E-Mail Verification | Parkla", $"Dear {user.Name}, Verification of account is necessary to registeration. You need to verify your account with the code below:\nVerification Code: {user.VerificationCode}").ConfigureAwait(false);
            throw new ParklaException("Verification code has sent to the email. Please check your e-mail", HttpStatusCode.OK);
        }

        var isVerifyPassword = BCrypt.Net.BCrypt.Verify(password, user.Password);
        if (!isVerifyPassword) throw notFound;

        return JwtHelper.GetTokens(user);
    }

    public async Task<bool> VerifyEmailCodeAsync(string username, string verificationCode, CancellationToken cancellationToken = default)
    {
        var notValid = new ParklaException("Username or verification code is not valid", HttpStatusCode.BadRequest);
        if(string.IsNullOrWhiteSpace(username) || username.Length > 30 || string.IsNullOrWhiteSpace(verificationCode))
            throw notValid;

        var user = await _userRepo.GetAsync(x => x.Username == username, cancellationToken).ConfigureAwait(false);
        if (user == null)
            throw new ParklaException("User with given username and password is not found", HttpStatusCode.NotFound);

        if (user.VerificationCode != verificationCode)
            return false;

        user.VerificationCode = null;
        user = await _userRepo.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        await SendEmailAsync(user, "Email Verified | Parkla", $"Dear {user.Name}, your Parkla account is verified.").ConfigureAwait(false);

        return true;
    }

    private async Task SendEmailAsync(User user, string subject, string text)
    {
        var message = new MailMessage();
        message.To.Add(new MailAddress(user.Email, $"{user.Name} {user.Surname}"));
        message.Subject = subject;
        message.Body = text;
        await _mailService.SendMailAsync(message).ConfigureAwait(false);
    }
}