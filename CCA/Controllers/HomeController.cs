using CCA.Data;
using CCA.Models;
using CCA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CCA.Controllers;

public class HomeController : Controller
{
    private readonly CCFADbContext _context = null!;

    public HomeController(CCFADbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        // 🔹 Trending Categories: Fetch actual artworks from portfolio by art style
        var allArtStyles = new[] {
            "Abstract Art", "Realism", "Impressionism", "Pop Art",
            "Minimalism", "Digital Art", "Oil Painting", "Watercolor"
        };

        var trendingCategories = new List<TrendingCategoryViewModel>();

        foreach (var style in allArtStyles)
        {
            // Find artists who have this art style
            var artists = await _context.Artists
                .Where(a => a.ArtStyles != null && a.ArtStyles.Contains(style))
                .ToListAsync();

            // Get all artworks from these artists
            var artworks = await _context.Artworks
                .Where(a => artists.Select(ar => ar.Id).Contains(a.ArtistId))
                .Include(a => a.Artist)
                .ThenInclude(ar => ar.ApplicationUser)
                .OrderByDescending(a => a.CreatedAt)
                .Take(4) // Get top 4 for grid display
                .ToListAsync();

            if (artworks.Any())
            {
                trendingCategories.Add(new TrendingCategoryViewModel
                {
                    Name = style,
                    ArtworkCount = artworks.Count,
                    ArtistCount = artists.Count,
                    FeaturedImageUrl = artworks.FirstOrDefault()?.ImageUrl,
                    SampleArtworks = artworks
                });
            }
        }

        // Order by artwork count (most popular first)
        trendingCategories = trendingCategories
            .OrderByDescending(c => c.ArtworkCount)
            .Take(6)
            .ToList();

        // 🔹 Recent Commissions: Only show if there are completed ones
        var recentCommissions = await _context.CommissionRequests
            .Include(cr => cr.Artist)
            .Include(cr => cr.Customer)
            .Where(cr => cr.Status == "Completed")
            .OrderByDescending(cr => cr.RespondedAt)
            .Take(6)
            .ToListAsync();

        var viewModel = new HomeIndexViewModel
        {
            TrendingCategories = trendingCategories,
            RecentCommissions = recentCommissions,
            ShowRecentCommissions = recentCommissions.Any(),
            TotalArtists = await _context.Artists.CountAsync(),
            TotalCommissions = await _context.CommissionRequests.CountAsync(cr => cr.Status == "Completed")
        };

        return View(viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}