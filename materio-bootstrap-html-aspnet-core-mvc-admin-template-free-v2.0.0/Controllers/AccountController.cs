using AspnetCoreMvcFull.Models.ViewModels;
using AspnetCoreMvcFull.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspnetCoreMvcFull.Models;


namespace AspnetCoreMvcFull.Controllers
{
  public class AccountController : Controller
  {
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IJwtService jwtService,
        ILogger<AccountController> logger)
    {
      _signInManager = signInManager;
      _userManager = userManager;
      _jwtService = jwtService;
      _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
      ViewData["ReturnUrl"] = returnUrl;
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
      ViewData["ReturnUrl"] = returnUrl;

      if (!ModelState.IsValid)
      {
        return View(model);
      }

      var user = await _userManager.FindByEmailAsync(model.Email);
      if (user == null)
      {
        ModelState.AddModelError(string.Empty, "Невірний email або пароль");
        return View(model);
      }

      var result = await _signInManager.PasswordSignInAsync(
          user.UserName!,
          model.Password,
          model.RememberMe,
          lockoutOnFailure: false);

      if (result.Succeeded)
      {
        _logger.LogInformation($"Користувач {user.Email} успішно увійшов");

        // Генерація JWT токену
        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);

        // Зберігаємо токен у cookie
        Response.Cookies.Append("AuthToken", token, new CookieOptions
        {
          HttpOnly = true,
          Secure = true,
          SameSite = SameSiteMode.Strict,
          Expires = DateTimeOffset.UtcNow.AddMinutes(60)
        });

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
          return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
      }

      if (result.IsLockedOut)
      {
        _logger.LogWarning($"Акаунт користувача {user.Email} заблокований");
        ModelState.AddModelError(string.Empty, "Акаунт заблокований");
        return View(model);
      }

      ModelState.AddModelError(string.Empty, "Невірний email або пароль");
      return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
      await _signInManager.SignOutAsync();
      Response.Cookies.Delete("AuthToken");
      _logger.LogInformation("Користувач вийшов з системи");
      return RedirectToAction("Login", "Account");
    }

    public IActionResult AccessDenied()
    {
      return View();
    }
  }
}
