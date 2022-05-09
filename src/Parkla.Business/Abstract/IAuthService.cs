using System.IdentityModel.Tokens.Jwt;
using Parkla.Core.DTOs;
using Parkla.Core.Entities;

namespace Parkla.Business.Abstract;
public interface IAuthService
{
    public Task<TokensDto> RefreshToken(string oldRefreshToken);
    public Task RevokeRefreshToken(string oldRefreshToken);
    public Task RegisterAsync(
        User user, 
        CancellationToken cancellationToken = default
    );

    public Task<TokensDto> LoginAsync(
        string username, 
        string password, 
        CancellationToken cancellationToken = default
    );

    public Task<bool> VerifyEmailCodeAsync(
        string username, 
        string verificationCode, 
        CancellationToken cancellationToken = default
    );
}