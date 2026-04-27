using System.Diagnostics;
using CCA.Models;
using Microsoft.AspNetCore.Mvc;

namespace CCA.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Mock Data for Trending Categories
            var categories = new List<string> { "Digital Illustration", "Oil Portraits", "Character Design", "Sculpture" };
            ViewBag.Categories = categories;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
