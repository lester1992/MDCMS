using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MDCMS.Server.Models;
namespace MDCMS.Server.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user, out DateTime expires);
    }
    public class JwtService : IJwtService
    {
        private readonly JwtOptions _opts;
        public JwtService(IOptions<JwtOptions> opts) => _opts = opts.Value;

        public string GenerateToken(User user, out DateTime expires)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id ?? ""),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("fullname", user.Name),
            new Claim("designation", user.Designation),
            new Claim("dateModified", user.DateModified.ToString("o"))
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opts.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            expires = DateTime.UtcNow.AddMinutes(_opts.ExpiresMinutes);

            var token = new JwtSecurityToken(
                issuer: _opts.Issuer,
                audience: _opts.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
