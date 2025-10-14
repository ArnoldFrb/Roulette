using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Roulette.Domain.Contracts.Security;
using Roulette.Infrastructure.Security.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Roulette.Infrastructure.Security.Services
{
    public class JwtTokenService(IOptions<JwtSettings> jwtOptions) : IJwtTokenService
    {
        private readonly JwtSettings _jwtSettings = jwtOptions.Value;
        public string GetJwtToken(string user, int id)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user),
                new Claim(ClaimTypes.Role, "Crupier"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
