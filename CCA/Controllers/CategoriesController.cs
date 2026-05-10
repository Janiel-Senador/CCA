using CCA.Data;
using CCA.Models;
using CCA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CCA.Controllers;

public class CategoriesController : Controller
{
    private readonly CCFADbContext _context = null!;

    public CategoriesController(CCFADbContext context) => _context = context;

    [HttpGet]
    [Route("categories/{style}")]
    public async Task<IActionResult> ByStyle(string style)
    {
        if (string.IsNullOrEmpty(style)) return NotFound();

        // Handle URL-encoded spaces (e.g., "Pop%20Art" -> "Pop Art")
        style = Uri.UnescapeDataString(style);

        // Find artists who list this style in their settings
        var artists = await _context.Artists
            .Include(a => a.ApplicationUser)
            .Include(a => a.Artworks)
            .Where(a => a.ArtStyles != null && a.ArtStyles.Contains(style))
            .ToListAsync();

        var artistsWithArtworks = new List<ArtistWithStyleArtworksViewModel>();

        foreach (var artist in artists)
        {
            // Filter ONLY artworks that match this exact style/category
            var filteredArtworks = artist.Artworks?
                .Where(a => a.Category == style)
                .OrderByDescending(a => a.CreatedAt)
                .ToList() ?? new List<Artwork>();

            // Only show artists who actually have artworks in this style
            if (filteredArtworks.Any())
            {
                artistsWithArtworks.Add(new ArtistWithStyleArtworksViewModel
                {
                    Artist = artist,
                    StyleArtworks = filteredArtworks
                });
            }
        }

        return View(new CategoryStyleViewModel
        {
            StyleName = style,
            Artists = artistsWithArtworks
        });
    }
}