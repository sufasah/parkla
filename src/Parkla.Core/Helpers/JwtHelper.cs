using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Parkla.Core.DTOs;
using Parkla.Core.Entities;

namespace Parkla.Core.Helpers
{
    public static class JwtHelper
    {
        public static byte[] SecretKey;
        public static readonly TokenValidationParameters TokenValidationParameters = new() {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(SecretKey),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        public static readonly JwtSecurityTokenHandler TokenHandler = new() {
            MaximumTokenSizeInBytes = 4000
        };

        public static TokensDto GetTokens(User user) {
            var dto = new TokensDto
            {
                AccessToken = GenerateAccessToken(user, out _),
                RefreshToken = GenerateRefreshToken(user.Id.ToString(), out var refreshToken),
                Expires = (int)(refreshToken.ValidTo - refreshToken.ValidFrom).TotalMinutes
            };
            return dto;
        }
        public static string GenerateAccessToken(User user, out JwtSecurityToken securityToken)
        {
            var tokenDescriptor = new SecurityTokenDescriptor {
                Claims = GetUserClaimsIdentity(user),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(5), // this will be changed later
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(SecretKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = TokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            securityToken = token;
            return TokenHandler.WriteToken(token);
        }

        public static string GenerateRefreshToken(string userId, out JwtSecurityToken securityToken)
        {
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new(new Claim[]{new(ClaimTypes.NameIdentifier, userId)}),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7), // this will be changed later
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(SecretKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = TokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            securityToken = token;
            return TokenHandler.WriteToken(token);
        }

        private static Dictionary<string, object> GetUserClaimsIdentity(User user)
        {
            var dic = new Dictionary<string, object>
            {
                { ClaimTypes.NameIdentifier, user.Id.ToString() },
                { ClaimTypes.UserData, user.Name },
                { ClaimTypes.Name, user.Name },
                { ClaimTypes.Surname, user.Surname },
                { ClaimTypes.Email, user.Email },
                { ClaimTypes.Country, "Turkey" },
                { ClaimTypes.DateOfBirth, user.Birthdate },
                { ClaimTypes.Gender, user.Gender },
                { ClaimTypes.GivenName, user.Email },
                { ClaimTypes.Email, user.Email },
                { ClaimTypes.MobilePhone, user.Phone },
                { ClaimTypes.StateOrProvince, user.District },
                { ClaimTypes.StreetAddress, user.Address }
            };
            return dic;
        }

        public static object GenerateAccessToken(User user, object _)
        {
            throw new NotImplementedException();
        }
    }
}