using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Models.Settings;

namespace AspnetCoreMvcFull.Services
{
  public class JwtService : IJwtService
  {
    private readonly JwtSettings _jwtSettings;

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
      _jwtSettings = jwtSettings.Value;
    }

    public string GenerateToken(AppUser user, IList<string> roles)
    {
      var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? "")
            };

      // Додаємо ролі до claims
      foreach (var role in roles)
      {
        claims.Add(new Claim(ClaimTypes.Role, role));
      }

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          issuer: _jwtSettings.Issuer,
          audience: _jwtSettings.Audience,
          claims: claims,
          expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
          signingCredentials: credentials
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
      try
      {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = true,
          ValidIssuer = _jwtSettings.Issuer,
          ValidateAudience = true,
          ValidAudience = _jwtSettings.Audience,
          ValidateLifetime = true,
          ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}
