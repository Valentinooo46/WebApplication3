using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

using static System.Net.Mime.MediaTypeNames;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        

        public  string Index()
        {
            return "HellO!";

        }
    }
    

}
