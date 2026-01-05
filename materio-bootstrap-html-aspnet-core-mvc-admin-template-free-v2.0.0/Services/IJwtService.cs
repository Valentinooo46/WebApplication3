using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Services
{
  public interface IJwtService
  {
    string GenerateToken(AppUser user, IList<string> roles, TimeSpan? lifetime = null);
    bool ValidateToken(string token);
  }
}
