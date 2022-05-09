using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Web.Models;
namespace Parkla.Web.Controllers;

[AllowAnonymous]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _service;
    private readonly IMapper _mapper;
    public AuthController(
        IAuthService service,
        IMapper mapper
    )
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet("refresh-token")]
    public async Task<IActionResult> RefreshToken() {
        var result = ValidateRefreshToken(out var tokenStr);
        if(result != null) return result;

        var tokens = await _service.RefreshToken(tokenStr!);
        return Ok(tokens);
    }

    [HttpGet("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken() {
        var result = ValidateRefreshToken(out var tokenStr);
        if(result != null) return result;

        await _service.RevokeRefreshToken(tokenStr!);
        return Ok("Refresh token has been revoked");
    }

    private IActionResult? ValidateRefreshToken(out string? tokenStr) {
        var authHeader = HttpContext.Request.Headers.Authorization.FirstOrDefault();
        tokenStr = null;
        if(authHeader == null)
            return BadRequest("Refresh token does not exist in authorization header");
        
        var badHeader = BadRequest("Authorization header is not an expected format or not a jwt bearer token");

        var split = authHeader.Split(" ");
        if(split.Length < 1) return badHeader;

        tokenStr = split[1];
        return null;   
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(UserDto dto, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<User>(dto);
        await _service.RegisterAsync(user, cancellationToken).ConfigureAwait(false);
        return Ok("User registered successfully. Verification e-mail has sent to the email");
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginDto dto)
    {
        var tokens = await _service.LoginAsync(dto.Username, dto.Password).ConfigureAwait(false);
        return Ok(tokens);
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyEmailCodeAsync(VerifyAccountDto dto, CancellationToken cancellationToken)
    {
        var verified = await _service.VerifyEmailCodeAsync(dto.Username, dto.VerificationCode, cancellationToken).ConfigureAwait(false);
        
        if(verified) return Ok("Email verified");
        else return BadRequest("Verification code is not correct");
    }

}