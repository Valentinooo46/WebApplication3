using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Models.ViewModels;
using AspnetCoreMvcFull.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace AspnetCoreMvcFull.Controllers
{
  public class AccountController : Controller
  {
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AccountController> _logger;
    private readonly IWebHostEnvironment _environment;

    public AccountController(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IJwtService jwtService,
        ILogger<AccountController> logger,
        IWebHostEnvironment environment)
    {
      _signInManager = signInManager;
      _userManager = userManager;
      _jwtService = jwtService;
      _logger = logger;
      _environment = environment;
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
        _logger.LogInformation("Користувач {Email} успішно увійшов", user.Email);

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);

        var isForwardedHttps = string.Equals(
            Request.Headers["X-Forwarded-Proto"],
            "https",
            StringComparison.OrdinalIgnoreCase);

        var isSecureRequest = Request.IsHttps || isForwardedHttps;

        var cookieOptions = new CookieOptions
        {
          HttpOnly = true,
          Secure = isSecureRequest,
          SameSite = isSecureRequest ? SameSiteMode.None : SameSiteMode.Lax,
          Expires = DateTimeOffset.UtcNow.AddMinutes(60)
        };

        Response.Cookies.Append("AuthToken", token, cookieOptions);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
          return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
      }

      if (result.IsLockedOut)
      {
        _logger.LogWarning("Акаунт користувача {Email} заблокований", user.Email);
        ModelState.AddModelError(string.Empty, "Акаунт заблок ований");
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
