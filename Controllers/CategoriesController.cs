using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.Services;

namespace WebApplication3.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _service.GetCategoriesAsync();
            return View(categories);
        }
        public  IActionResult Create()
        {
            return View();
        }



        public async Task<IActionResult> Details(int id)
        {
            var category = await _service.GetCategoryAsync(id);
            return View(category);
        }
        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _service.GetCategoryAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateCategoryAsync(category);
                return RedirectToAction("Index");
            }
            return View(category);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            await _service.EditCategoryAsync(category);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.RemoveCategoryAsync(id);
            return RedirectToAction("Index");
        }
    }
}
