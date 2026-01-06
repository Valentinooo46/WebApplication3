using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Models.ViewModels;
using AspnetCoreMvcFull.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Web;


namespace AspnetCoreMvcFull.Controllers
{
  public class AccountController : Controller
  {
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AccountController> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly IEmailService _emailService;

    public AccountController(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IJwtService jwtService,
        ILogger<AccountController> logger,
        IWebHostEnvironment environment,
        IEmailService emailService)
    {
      _signInManager = signInManager;
      _userManager = userManager;
      _jwtService = jwtService;
      _logger = logger;
      _environment = environment;
      _emailService = emailService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
      ViewData["ReturnUrl"] = returnUrl;
      return View();
    }

    [HttpPost]
    [AllowAnonymous]
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

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
      return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
      if (!ModelState.IsValid) return View(model);

      var user = await _userManager.FindByEmailAsync(model.Email);
      if (user == null)
      {
        // Do not reveal that the user does not exist
        return RedirectToAction(nameof(ForgotPasswordConfirmation));
      }

      var token = await _userManager.GeneratePasswordResetTokenAsync(user);
      var callbackUrl = Url.Action("ResetPassword", "Account", new { token = HttpUtility.UrlEncode(token), email = model.Email }, protocol: Request.Scheme);

      var body = $"<p>Щоб скинути пароль, натисніть <a href=\"{callbackUrl}\">тут</a>.</p>";

      await _emailService.SendEmailAsync(model.Email, "Reset Password", body);

      return RedirectToAction(nameof(ForgotPasswordConfirmation));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation()
    {
      return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string token, string email)
    {
      _logger.LogInformation("ResetPassword GET called with email={Email}", email);
      var model = new ResetPasswordViewModel { Token = token, Email = email };
      return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
      if (!ModelState.IsValid) return View(model);

      var user = await _userManager.FindByEmailAsync(model.Email);
      if (user == null)
      {
        // Do not reveal that the user does not exist
        return RedirectToAction(nameof(ResetPasswordConfirmation));
      }

      var decodedToken = HttpUtility.UrlDecode(model.Token);
      var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);
      if (result.Succeeded)
      {
        return RedirectToAction(nameof(ResetPasswordConfirmation));
      }

      foreach (var error in result.Errors)
      {
        ModelState.AddModelError(string.Empty, error.Description);
      }

      return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation()
    {
      return View();
    }
  }
}
