using AspnetCoreMvcFull.Services;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Middleware
{
  public class JwtCookieMiddleware
  {
    private readonly RequestDelegate _next;

    public JwtCookieMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context,
        IJwtService jwtService,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager)
    {
      var path = context.Request.Path.Value?.ToLower();

      // Пропускаємо сторінки автентифікації та статичні файли
      if (path != null && (path.StartsWith("/account/") ||
          path.StartsWith("/css/") ||
          path.StartsWith("/js/") ||
          path.StartsWith("/vendor/")))
      {
        await _next(context);
        return;
      }

      // Перевіряємо наявність токену в cookie
      if (context.Request.Cookies.TryGetValue("AuthToken", out var token))
      {
        if (jwtService.ValidateToken(token))
        {
          // Токен валідний, продовжуємо
          var tokenHandler = new JwtSecurityTokenHandler();
          var jwtToken = tokenHandler.ReadJwtToken(token);
          var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

          if (!string.IsNullOrEmpty(userId))
          {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null && !await signInManager.UserManager.IsLockedOutAsync(user))
            {
              await _next(context);
              return;
            }
          }
        }
      }

      // Якщо токену немає або він невалідний - редірект на Login
      context.Response.Redirect($"/Account/Login?returnUrl={Uri.EscapeDataString(context.Request.Path + context.Request.QueryString)}");
    }
  }
}
