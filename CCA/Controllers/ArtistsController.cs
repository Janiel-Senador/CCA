using CCA.ViewModels;
using CCA.Data;
using CCA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CCA.Controllers;

public class ArtistsController : Controller
{
    private readonly CCFADbContext _context = null!;

    public ArtistsController(CCFADbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> Browse(string? search)
    {
        var query = _context.Artists
            .Include(a => a.ApplicationUser)
            .Include(a => a.CommissionTiers)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(a => a.Name.Contains(search) || a.Bio.Contains(search) || (a.ArtStyles != null && a.ArtStyles.Contains(search)));

        return View(await query.ToListAsync());
    }

    [HttpGet]
    [Route("Artists/Profile")]
    public async Task<IActionResult> Profile(string? slug)
    {
        if (string.IsNullOrEmpty(slug)) return NotFound();

        var artist = await _context.Artists
            .Include(a => a.ApplicationUser)
            .Include(a => a.Artworks)
            .Include(a => a.CommissionTiers)
            .FirstOrDefaultAsync(a => a.Slug == slug);

        if (artist == null) return NotFound();

        return View(new ArtistProfileViewModel
        {
            Id = artist.Id,
            Name = artist.Name ?? "Unknown Artist",
            Slug = artist.Slug ?? string.Empty,
            PortraitUrl = artist.ApplicationUser?.PortraitUrl,
            IsVerified = artist.ApplicationUser?.IsVerified ?? false,
            Bio = artist.Bio,
            Location = artist.Location,
            Rating = artist.Rating,
            ResponseTime = artist.ResponseTime,
            ArtStyles = artist.ArtStyles,
            Portfolio = artist.Artworks?.OrderByDescending(a => a.CreatedAt).ToList(),
            Rates = artist.CommissionTiers?.OrderBy(t => t.Price).ToList(),
            ApplicationUser = artist.ApplicationUser // ✅ Maps correctly
        });
    }
}