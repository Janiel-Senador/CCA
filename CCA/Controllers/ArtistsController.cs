using Microsoft.AspNetCore.Mvc;

namespace CCA.Controllers
{
    public class ArtistsController : Controller
    {
        // GET: /Artists/Browse - Marketplace page
        public IActionResult Browse()
        {
            return View();
        }

        // GET: /Artist/{slug} - Artist Profile page
        public IActionResult Profile(string slug)
        {
            // For now, just return the same profile view for any slug
            return View();
        }
    }
}