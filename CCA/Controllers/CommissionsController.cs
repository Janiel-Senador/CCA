using CCA.ViewModels;
using CCA.Data;
using CCA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace CCA.Controllers;

public class CommissionsController : Controller
{
    private readonly CCFADbContext _context = null!;
    private readonly UserManager<ApplicationUser> _userManager = null!;

    public CommissionsController(CCFADbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ✅ GET: Show commission request form
    [HttpGet]
    [Route("commission/request/{artistId:int}")]
    public async Task<IActionResult> Create(int artistId)
    {
        var artist = await _context.Artists
            .Include(a => a.CommissionTiers)
            .FirstOrDefaultAsync(a => a.Id == artistId);

        if (artist == null) return NotFound();

        var minPrice = artist.CommissionTiers?.Min(t => (decimal?)t.Price) ?? 100m;

        return View(new CommissionRequestViewModel
        {
            ArtistId = artistId,
            ArtistName = artist.Name ?? "Unknown Artist",
            BasePrice = minPrice,
            Total = minPrice,
            AvailableTiers = artist.CommissionTiers?
                .Select(t => new SelectListItem
                {
                    Value = t.Price.ToString(),
                    Text = $"{t.Name} - ${t.Price:F0}"
                })
                .ToList() ?? new List<SelectListItem>()
        });
    }

    // ✅ POST: Handle form submission
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("commission/request/submit")]
    public async Task<IActionResult> Submit(CommissionRequestViewModel model)
    {
        var customer = await _userManager.GetUserAsync(User);
        if (customer == null) return Challenge();

        var artist = await _context.Artists.FirstOrDefaultAsync(a => a.Id == model.ArtistId);
        if (artist == null) return NotFound();

        var request = new CommissionRequest
        {
            ArtistId = model.ArtistId,
            CustomerId = customer.Id,
            Description = model.Description?.Trim() ?? string.Empty,
            BasePrice = model.BasePrice,
            TotalPrice = model.Total,
            TurnaroundOption = model.TurnaroundOption ?? "standard",
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.CommissionRequests.Add(request);
        await _context.SaveChangesAsync();

        return RedirectToAction("Confirmation", new { id = request.Id });
    }

    // ✅ GET: Show confirmation page
    [HttpGet]
    [Route("commission/confirmation/{id:int}")]
    public async Task<IActionResult> Confirmation(int id)
    {
        var request = await _context.CommissionRequests
            .Include(r => r.Artist)
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null) return NotFound();

        return View(request);
    }
}