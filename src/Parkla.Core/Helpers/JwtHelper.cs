using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Parkla.Core.DTOs;
using Parkla.Core.Entities;

namespace Parkla.Core.Helpers
{
    public static class JwtHelper
    {
        public static byte[] SecretKey;
        public static TokenValidationParameters TokenValidationParameters;

        public static readonly JwtSecurityTokenHandler TokenHandler = new() {
            MaximumTokenSizeInBytes = 4000
        };

        public static TokensDto GetTokens(User user, out JwtSecurityToken accessToken, out JwtSecurityToken refreshToken) {
            var dto = new TokensDto
            {
                AccessToken = GenerateAccessToken(user, out accessToken),
                RefreshToken = GenerateRefreshToken(user.Id.ToString(), out refreshToken),
                Expires = (int)(refreshToken.ValidTo - refreshToken.ValidFrom).TotalMinutes
            };
            return dto;
        }
        public static string GenerateAccessToken(User user, out JwtSecurityToken securityToken)
        {
            var tokenDescriptor = new SecurityTokenDescriptor {
                Claims = GetUserClaimsIdentity(user),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(1), // this will be changed later
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(SecretKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = TokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            securityToken = token;
            return TokenHandler.WriteToken(token);
        }

        public static string GenerateRefreshToken(string userId, out JwtSecurityToken securityToken)
        {
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new(new Claim[]{
                    new(JwtRegisteredClaimNames.Sub, userId)
                }),
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
                { ClaimTypes.Name, user.Name },
                { ClaimTypes.Surname, user.Surname },
                { ClaimTypes.Email, user.Email },
                { ClaimTypes.DateOfBirth, user.Birthdate },
                { ClaimTypes.Gender, user.Gender },
                { ClaimTypes.MobilePhone, user.Phone },
                { ClaimTypes.Locality, user.CityId },
                { ClaimTypes.StateOrProvince, user.DistrictId },
                { ClaimTypes.StreetAddress, user.Address },
                { JwtRegisteredClaimNames.Sub, user.Id.ToString() },
                { "preferred_username", user.Username },
                { JwtRegisteredClaimNames.Name, user.Name },
                { JwtRegisteredClaimNames.FamilyName, user.Surname },
                { JwtRegisteredClaimNames.Email, user.Email },
                { JwtRegisteredClaimNames.Birthdate, user.Birthdate },
                { JwtRegisteredClaimNames.Gender, user.Gender },
                { "phone_number", user.Phone },
                { "address", JsonSerializer.Serialize(new{
                    locality = user.CityId,
                    region = user.DistrictId,
                    street_address = user.Address
                }) },
            };
            return dic;
        }

        public static object GenerateAccessToken(User user, object _)
        {
            throw new NotImplementedException();
        }
    }
}