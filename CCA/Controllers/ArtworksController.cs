using CCA.Data;
using CCA.Models;
using CCA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CCA.Controllers;

public class ArtworksController : Controller
{
    private readonly CCFADbContext _context = null!;

    public ArtworksController(CCFADbContext context) => _context = context;

    // ✅ GET: Show artworks filtered by category/style
    [HttpGet]
    [Route("artworks/category/{category}")]
    public async Task<IActionResult> ByCategory(string category)
    {
        if (string.IsNullOrEmpty(category)) return NotFound();

        // Find artists whose ArtStyles contain the category
        var artists = await _context.Artists
            .Include(a => a.ApplicationUser)
            .Include(a => a.Artworks)
            .Where(a => a.ArtStyles != null && a.ArtStyles.Contains(category))
            .ToListAsync();

        // Flatten all artworks from these artists that match the category
        var artworks = artists
            .SelectMany(a => a.Artworks ?? new List<Artwork>())
            .Where(a => a.Category == category || a.Medium?.Contains(category) == true || a.Description?.Contains(category) == true)
            .OrderByDescending(a => a.CreatedAt)
            .ToList();

        var viewModel = new ArtworksByCategoryViewModel
        {
            Category = category,
            Artworks = artworks,
            ArtistCount = artists.Count
        };

        return View(viewModel);
    }

    // ✅ GET: View single artwork details
    [HttpGet]
    [Route("artworks/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var artwork = await _context.Artworks
            .Include(a => a.Artist)
            .ThenInclude(ar => ar.ApplicationUser)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (artwork == null) return NotFound();

        return View(artwork);
    }
}