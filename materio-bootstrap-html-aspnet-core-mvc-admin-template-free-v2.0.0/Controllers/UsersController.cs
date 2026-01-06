using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspnetCoreMvcFull.Controllers
{
 [Authorize]
 public class UsersController : Controller
 {
 private readonly ApplicationDbContext _dbContext;

 public UsersController(ApplicationDbContext dbContext)
 {
 _dbContext = dbContext;
 }

 [HttpGet]
 public async Task<IActionResult> Search(string q)
 {
 var model = new UsersSearchViewModel
 {
 Query = q ?? string.Empty,
 Results = new List<UserListItemViewModel>()
 };

 if (!string.IsNullOrWhiteSpace(q))
 {
 var term = q.ToLower();
 model.Results = await _dbContext.Users
 .AsNoTracking()
 .Where(u => u.Email != null && u.Email.ToLower().Contains(term))
 .OrderBy(u => u.Email)
 .Select(u => new UserListItemViewModel
 {
 Id = u.Id,
 Email = u.Email,
 UserName = u.UserName
 })
 .ToListAsync();
 }

 return View(model);
 }

 [HttpGet]
 public async Task<IActionResult> Profile(string id)
 {
 if (string.IsNullOrEmpty(id)) return NotFound();

 var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
 if (user == null) return NotFound();

 var model = new UserProfileViewModel
 {
 Id = user.Id,
 Email = user.Email,
 UserName = user.UserName,
 PhoneNumber = user.PhoneNumber
 };

 return View(model);
 }
 }
}
