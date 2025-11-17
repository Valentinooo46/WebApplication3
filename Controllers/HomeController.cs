using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using WebApplication3.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        //    private readonly ILogger<HomeController> _logger;
        //    private readonly NewContext _context;
        //    public static int count = 0;
        //    static object _lock = new object();
        //    public HomeController(ILogger<HomeController> logger,NewContext context)
        //    {
        //        _logger = logger;
        //        _context = context;
        //    }

        public async Task<string> Index()
        {
            return "HellO!";

        }
    }
    //    [HttpGet]
    //    public IActionResult Upload()
    //    {
    //        return View();
    //    }
    //    [HttpPost]
    //    public async Task<IActionResult> Upload(IFormFile photo, string title, string summary, string slug,string content)
    //    {
    //        if (photo == null || photo.Length == 0)
    //        {
    //            return BadRequest("Фото не вибрано.");
    //        }
    //        // Шлях до папки для збереження
    //        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
    //        if (!Directory.Exists(uploadsFolder))
    //        {
    //            Directory.CreateDirectory(uploadsFolder);
    //        }

    //        // Збереження оригінального файлу
    //        string originalFilePath = Path.Combine(uploadsFolder, photo.FileName);
    //        using (var stream = new FileStream(originalFilePath, FileMode.Create))
    //        {
    //            await photo.CopyToAsync(stream);
    //        }
    //        // Створення зображень різних розмірів
    //        string[] sizes = { "small", "medium", "large" };
    //        int[] dimensions = { 100, 500, 1000 }; // Розміри для кожного варіанту
    //        var photoObject = new New
    //        {
    //            title = title,
    //            summary = summary,
    //            slug = slug,
    //            content = content,
    //            image = Path.Combine("uploads", sizes[0], Path.GetFileNameWithoutExtension(photo.FileName) + ".webp").Replace("\\", "/")

    //        };
    //        for (int i = 0; i < sizes.Length; i++)
    //        {
    //            string sizeFolder = Path.Combine(uploadsFolder, sizes[i]);
    //            if (!Directory.Exists(sizeFolder))
    //            {
    //                Directory.CreateDirectory(sizeFolder);
    //            }

    //            string resizedFilePath = Path.Combine(sizeFolder, Path.GetFileNameWithoutExtension(photo.FileName)+ ".webp");

    //            using (var image = await SixLabors.ImageSharp.Image.LoadAsync(originalFilePath))
    //            {
    //                image.Mutate(x => { x.Resize(dimensions[i], 0);} ); // Масштабування по ширині, висота автоматична
    //                await image.SaveAsWebpAsync(resizedFilePath);
    //            }
    //        }
    //        // Створення об'єкта для збереження в базі даних



    //        // Збереження у базу даних
    //        await _context.NewModels.AddAsync(photoObject);
    //        await _context.SaveChangesAsync();
    //        lock (_lock)
    //        {
    //            count++;

    //        }
    //        return Ok($"Об'єкт збережено: {title}");

    //    }

    //    public IActionResult Privacy()
    //    {
    //        return View();
    //    }
    //    public async Task<IEnumerable<New>> AddNewsAndReturnListAsync()
    //    {
    //        for (int i = 0; i < 10; i++)
    //        {

    //            await _context.NewModels.AddAsync(_context.NewFaker.Generate());
    //        }
    //        await _context.SaveChangesAsync();
    //        return _context.NewModels.ToList();

    //    }


    //    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    //    public IActionResult Error()
    //    {
    //        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    //    }
    //}

}
