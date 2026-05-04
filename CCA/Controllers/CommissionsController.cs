using Microsoft.AspNetCore.Mvc;

namespace CCA.Controllers
{
    public class CommissionsController : Controller
    {
        // GET: /Commissions/Create - Commission Request Form
        public IActionResult Create(string artistSlug)
        {
            // Pass artist name to view for display (front-end only)
            ViewBag.ArtistName = !string.IsNullOrEmpty(artistSlug) ? "Evelyn Vance" : "Select an Artist";
            return View();
        }

        // POST: /Commissions/Create - Just redirects for demo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create()
        {
            // Front-end demo: just redirect to a "success" message
            return RedirectToAction("Confirmation");
        }

        // GET: /Commissions/Confirmation - Simple success page
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}