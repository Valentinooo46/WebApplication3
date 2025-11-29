using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication3.Models;
using WebApplication3.Services;

namespace WebApplication3.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 AppDbContext appDbContext,
                                 IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = appDbContext;
            _emailSender = emailSender;
        }
        
        public IActionResult Index()
        {
            var products = _context.Products
           .Include(p => p.Images)
           .ToList();

            return View(products);

        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.FullName };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Невдала спроба входу.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Не показуємо, що користувач не існує
                return View("ForgotPasswordConfirmation");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account",
                new { userId = user.Id, code = code }, protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(model.Email,
                "Відновлення паролю",
                $"Для відновлення паролю перейдіть за <a href='{callbackUrl}'>посиланням</a>");

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        public IActionResult ForgotPasswordConfirmation() => View();

        
        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null) return BadRequest("Необхідний код для скидання паролю.");
            return View(new ResetPasswordViewModel { Code = code });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return RedirectToAction("ResetPasswordConfirmation");

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
                return RedirectToAction("ResetPasswordConfirmation");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        public IActionResult ResetPasswordConfirmation() => View();



        //public async Task<IActionResult> EditRoles(string id)
        //{
        //    var user = await _userManager.FindByIdAsync(id);
        //    if (user == null) return NotFound();

        //    var allRoles = _roleManager.Roles.ToList();
        //    var userRoles = await _userManager.GetRolesAsync(user);

        //    var model = new EditUserRolesViewModel
        //    {
        //        UserId = user.Id,
        //        UserName = user.UserName,
        //        Roles = allRoles.Select(r => new RoleSelection
        //        {
        //            RoleName = r.Name,
        //            Selected = userRoles.Contains(r.Name)
        //        }).ToList()
        //    };

        //    return View(model);
        //}


        //[HttpPost]
        //public async Task<IActionResult> EditRoles(EditUserRolesViewModel model)
        //{
        //    var user = await _userManager.FindByIdAsync(model.UserId);
        //    if (user == null) return NotFound();

        //    var currentRoles = await _userManager.GetRolesAsync(user);

        //    // Забираємо всі ролі
        //    await _userManager.RemoveFromRolesAsync(user, currentRoles);

        //    // Додаємо вибрані
        //    var selectedRoles = model.Roles.Where(r => r.Selected).Select(r => r.RoleName);
        //    await _userManager.AddToRolesAsync(user, selectedRoles);

        //    return RedirectToAction("Index");
        //}


    }
}
