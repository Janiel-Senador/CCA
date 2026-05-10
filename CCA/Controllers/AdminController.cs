using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CCA.Data;
using CCA.Models;

namespace CCA.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly CCFADbContext _context = null!;
    private readonly UserManager<ApplicationUser> _userManager = null!;

    public AdminController(CCFADbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ✅ GET: List of artists pending verification
    [HttpGet]
    [Route("Admin/PendingArtists")]
    public async Task<IActionResult> PendingArtists()
    {
        var pendingArtists = await _context.Artists
            .Include(a => a.ApplicationUser)
            .Include(a => a.Artworks)
            .Include(a => a.CommissionTiers)
            .Where(a => a.ApplicationUser != null && !a.ApplicationUser.IsVerified)
            .OrderByDescending(a => a.MemberSince)
            .ToListAsync();

        return View(pendingArtists);
    }

    // ✅ POST: Approve or decline artist verification
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Admin/VerifyArtist")]
    public async Task<IActionResult> VerifyArtist(string userId, bool approve, string? reason)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        if (approve)
        {
            user.IsVerified = true;
            user.VerifiedAt = DateTime.UtcNow;

            var artist = await _context.Artists.FirstOrDefaultAsync(a => a.ApplicationUserId == userId);
            if (artist != null)
            {
                artist.IsVerified = true;
                _context.Artists.Update(artist);
            }

            await _userManager.UpdateAsync(user);
            TempData["Success"] = $"✓ {user.FullName} is now a verified artist!";
        }
        else
        {
            TempData["Info"] = $"✗ Verification declined for {user.FullName}";
        }

        return RedirectToAction("PendingArtists");
    }

    // ✅ POST: Delete a user (NEW - Fixes 404)
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Admin/DeleteUser")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return BadRequest();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        // Prevent deleting yourself or other admins
        var currentUser = await _userManager.GetUserAsync(User);
        if (user.Id == currentUser?.Id || user.UserRole == "Admin")
        {
            TempData["Error"] = "Cannot delete admin accounts or your own account.";
            return RedirectToAction("Users");
        }

        // Delete related Artist data if exists
        if (user.UserRole == "Artist")
        {
            var artist = await _context.Artists.FirstOrDefaultAsync(a => a.ApplicationUserId == userId);
            if (artist != null)
            {
                // Delete related data first (foreign key constraints)
                _context.CommissionTiers.RemoveRange(_context.CommissionTiers.Where(t => t.ArtistId == artist.Id));
                _context.Artworks.RemoveRange(_context.Artworks.Where(a => a.ArtistId == artist.Id));
                _context.CommissionRequests.RemoveRange(_context.CommissionRequests.Where(cr => cr.ArtistId == artist.Id));

                _context.Artists.Remove(artist);
            }
        }

        // Delete related CommissionRequests as customer
        _context.CommissionRequests.RemoveRange(_context.CommissionRequests.Where(cr => cr.CustomerId == userId));

        // Delete the user
        var deleteResult = await _userManager.DeleteAsync(user);
        if (!deleteResult.Succeeded)
        {
            TempData["Error"] = "Failed to delete user: " + string.Join(", ", deleteResult.Errors.Select(e => e.Description));
            return RedirectToAction("Users");
        }

        TempData["Success"] = $"User {user.FullName} has been deleted.";
        return RedirectToAction("Users");
    }

    // ✅ GET: Manage all users
    [HttpGet]
    [Route("Admin/Users")]
    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users
            .OrderByDescending(u => u.EmailConfirmed)
            .ThenBy(u => u.FullName)
            .ToListAsync();

        return View(users);
    }
}