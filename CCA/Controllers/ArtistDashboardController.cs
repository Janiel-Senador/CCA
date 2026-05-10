using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CCA.Data;
using CCA.Models;
using CCA.ViewModels;

namespace CCA.Controllers;

[Authorize]
public class ArtistDashboardController : Controller
{
    private readonly CCFADbContext _context = null!;
    private readonly UserManager<ApplicationUser> _userManager = null!;
    private readonly IWebHostEnvironment _env = null!;

    public ArtistDashboardController(
        CCFADbContext context,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment env)
    {
        _context = context;
        _userManager = userManager;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.UserRole != "Artist") return Forbid();

        var artist = await _context.Artists
            .Include(a => a.CommissionTiers)
            .FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id);

        if (artist == null) return NotFound();

        var earnings = await _context.CommissionRequests
            .Where(cr => cr.ArtistId == artist.Id && cr.Status == "Completed")
            .SumAsync(cr => (decimal?)cr.TotalPrice);

        return View(new ArtistDashboardViewModel
        {
            ArtistName = artist.Name ?? "Unknown Artist",
            IsVerified = user.IsVerified,
            TotalEarnings = earnings ?? 0m,
            PendingRequests = await _context.CommissionRequests.CountAsync(cr => cr.ArtistId == artist.Id && cr.Status == "Pending"),
            ActiveCommissions = await _context.CommissionRequests.CountAsync(cr => cr.ArtistId == artist.Id && cr.Status == "Accepted"),
            PortfolioCount = await _context.Artworks.CountAsync(a => a.ArtistId == artist.Id),
            RecentCommissions = new List<CommissionSummaryViewModel>()
        });
    }

    [HttpGet]
    [Route("ArtistDashboard/Commissions")]
    public async Task<IActionResult> Commissions()
    {
        var artist = await GetCurrentUserArtistAsync();
        if (artist == null) return NotFound();
        return View(await _context.CommissionRequests.Include(cr => cr.Customer).Where(cr => cr.ArtistId == artist.Id).OrderByDescending(cr => cr.CreatedAt).ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("ArtistDashboard/Commissions/Accept/{id:int}")]
    public async Task<IActionResult> AcceptCommission(int id)
    {
        var artist = await GetCurrentUserArtistAsync();
        var req = await _context.CommissionRequests.FirstOrDefaultAsync(cr => cr.Id == id && cr.ArtistId == artist!.Id);
        if (req == null) return NotFound();
        req.Status = "Accepted"; req.RespondedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        TempData["Success"] = "Commission accepted.";
        return RedirectToAction("Commissions");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("ArtistDashboard/Commissions/Decline/{id:int}")]
    public async Task<IActionResult> DeclineCommission(int id, string? reason)
    {
        var artist = await GetCurrentUserArtistAsync();
        var req = await _context.CommissionRequests.FirstOrDefaultAsync(cr => cr.Id == id && cr.ArtistId == artist!.Id);
        if (req == null) return NotFound();
        req.Status = "Declined"; req.RespondedAt = DateTime.UtcNow; req.ArtistResponse = reason?.Trim();
        await _context.SaveChangesAsync();
        return RedirectToAction("Commissions");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("ArtistDashboard/Commissions/Complete/{id:int}")]
    public async Task<IActionResult> MarkAsCompleted(int id)
    {
        var artist = await GetCurrentUserArtistAsync();
        if (artist == null) return NotFound();
        var req = await _context.CommissionRequests.FirstOrDefaultAsync(cr => cr.Id == id && cr.ArtistId == artist.Id);
        if (req == null) return NotFound();
        if (req.Status != "Accepted") { TempData["Error"] = "Only accepted commissions can be completed."; return RedirectToAction("Commissions"); }
        req.Status = "Completed"; req.RespondedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        TempData["Success"] = "Commission marked as completed.";
        return RedirectToAction("Commissions");
    }

    [HttpGet]
    public async Task<IActionResult> ManageTiers()
    {
        var artist = await GetCurrentUserArtistAsync();
        if (artist == null) return NotFound();
        return View(await _context.CommissionTiers.Where(t => t.ArtistId == artist.Id).OrderBy(t => t.Price).ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTier(string? name, decimal price, string? description)
    {
        var artist = await GetCurrentUserArtistAsync();
        if (artist == null || string.IsNullOrWhiteSpace(name)) return BadRequest();
        _context.CommissionTiers.Add(new CommissionTier { ArtistId = artist.Id, Name = name.Trim(), Price = price, Description = description?.Trim() ?? string.Empty });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ManageTiers));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateTier(int tierId, decimal price, string? description)
    {
        var artist = await GetCurrentUserArtistAsync();
        var tier = await _context.CommissionTiers.FirstOrDefaultAsync(t => t.Id == tierId && t.ArtistId == artist!.Id);
        if (tier == null) return NotFound();
        tier.Price = price; tier.Description = description?.Trim();
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ManageTiers));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTier(int tierId)
    {
        var artist = await GetCurrentUserArtistAsync();
        var tier = await _context.CommissionTiers.FirstOrDefaultAsync(t => t.Id == tierId && t.ArtistId == artist!.Id);
        if (tier == null) return NotFound();
        _context.CommissionTiers.Remove(tier);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ManageTiers));
    }

    [HttpGet]
    public async Task<IActionResult> EditSettings()
    {
        var artist = await GetCurrentUserArtistAsync();
        if (artist == null) return NotFound();
        return View(new ArtistSettingsViewModel
        {
            Bio = artist.Bio,
            Location = artist.Location,
            ResponseTime = artist.ResponseTime,
            SelectedStyles = artist.ArtStyles?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? new(),
            AllStyles = new[] { "Abstract Art", "Realism", "Impressionism", "Pop Art", "Minimalism", "Digital Art", "Oil Painting", "Watercolor" }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSettings(ArtistSettingsViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var artist = await GetCurrentUserArtistAsync();
        if (artist == null) return NotFound();
        artist.Bio = model.Bio?.Trim() ?? artist.Bio;
        artist.Location = model.Location?.Trim() ?? artist.Location;
        artist.ResponseTime = model.ResponseTime ?? "Under 48h";
        artist.ArtStyles = model.SelectedStyles?.Any() == true ? string.Join(", ", model.SelectedStyles) : null;
        await _context.SaveChangesAsync();
        TempData["Success"] = "Settings updated.";
        return RedirectToAction(nameof(EditSettings));
    }

    [HttpGet]
    public async Task<IActionResult> Portfolio()
    {
        var artist = await GetCurrentUserArtistAsync();
        if (artist == null) return NotFound();
        return View(await _context.Artworks.Where(a => a.ArtistId == artist.Id).OrderByDescending(a => a.CreatedAt).ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadArtwork(IFormFile? file, string? title, string? description, string? category, string? medium, int? year)
    {
        if (file == null || file.Length == 0 || string.IsNullOrWhiteSpace(title)) return BadRequest("File and title are required.");
        var artist = await GetCurrentUserArtistAsync();
        if (artist == null) return NotFound();

        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var uploadsFolder = Path.Combine(webRoot, "uploads", "artworks");
        Directory.CreateDirectory(uploadsFolder);

        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{artist.Id}_{Guid.NewGuid().ToString("N")[..8]}{ext}";
        using var stream = new FileStream(Path.Combine(uploadsFolder, fileName), FileMode.Create);
        await file.CopyToAsync(stream);

        _context.Artworks.Add(new Artwork
        {
            ArtistId = artist.Id,
            Title = title?.Trim() ?? string.Empty,
            ImageUrl = $"/uploads/artworks/{fileName}",
            Description = description?.Trim() ?? string.Empty,
            Category = category?.Trim() ?? string.Empty,
            Medium = medium?.Trim() ?? string.Empty,
            Year = year.HasValue ? year.Value : (int?)null,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        TempData["Success"] = "Artwork uploaded.";
        return RedirectToAction(nameof(Portfolio));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteArtwork(int artworkId)
    {
        var artist = await GetCurrentUserArtistAsync();
        if (artist == null) return NotFound();
        var art = await _context.Artworks.FirstOrDefaultAsync(a => a.Id == artworkId && a.ArtistId == artist.Id);
        if (art == null) return NotFound();
        var fullPath = Path.Combine(_env.WebRootPath ?? Directory.GetCurrentDirectory(), "wwwroot", art.ImageUrl?.TrimStart('/') ?? string.Empty);
        if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
        _context.Artworks.Remove(art);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Portfolio));
    }

    private async Task<Artist?> GetCurrentUserArtistAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        return user?.UserRole == "Artist" ? await _context.Artists.FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id) : null;
    }
}