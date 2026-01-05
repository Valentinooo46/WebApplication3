using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Services
{
  public interface IJwtService
  {
    string GenerateToken(AppUser user, IList<string> roles);
    bool ValidateToken(string token);
  }
}
