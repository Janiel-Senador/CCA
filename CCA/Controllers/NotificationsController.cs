using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CCA.Data;
using CCA.Models;
using CCA.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CCA.Controllers;

[Authorize]
public class NotificationsController : Controller
{
    private readonly CCFADbContext _context = null!;
    private readonly UserManager<ApplicationUser> _userManager = null!;

    public NotificationsController(CCFADbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var notifications = new List<NotificationViewModel>();

        if (user.UserRole == "Artist")
        {
            var artist = await _context.Artists.FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id);
            if (artist != null)
            {
                var newRequests = await _context.CommissionRequests
                    .Include(cr => cr.Customer)
                    .Where(cr => cr.ArtistId == artist.Id && cr.Status == "Pending")
                    .OrderByDescending(cr => cr.CreatedAt)
                    .Take(10)
                    .ToListAsync();

                foreach (var req in newRequests)
                {
                    notifications.Add(new NotificationViewModel
                    {
                        Id = req.Id,
                        Type = "CommissionRequest",
                        Title = "New Commission Request",
                        Message = $"{req.Customer?.FullName ?? "A customer"} requested a commission: {(req.Description?.Length > 50 ? req.Description?.Substring(0, 50) + "..." : req.Description)}",
                        Timestamp = req.CreatedAt,
                        IsRead = false,
                        ActionUrl = "/ArtistDashboard/Commissions"
                    });
                }
            }
        }
        else if (user.UserRole == "Customer")
        {
            var myRequests = await _context.CommissionRequests
                .Include(cr => cr.Artist)
                .Where(cr => cr.CustomerId == user.Id && cr.RespondedAt != null)
                .OrderByDescending(cr => cr.RespondedAt)
                .Take(10)
                .ToListAsync();

            foreach (var req in myRequests)
            {
                notifications.Add(new NotificationViewModel
                {
                    Id = req.Id,
                    Type = "CommissionUpdate",
                    Title = $"Commission {req.Status}",
                    Message = $"Your request to {req.Artist?.Name ?? "an artist"} has been {req.Status?.ToLower() ?? "updated"}.",
                    Timestamp = req.RespondedAt ?? req.CreatedAt,
                    IsRead = false,
                    ActionUrl = $"/customer/commissions/{req.Id}"
                });
            }
        }

        return View(notifications.OrderByDescending(n => n.Timestamp).ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllRead()
    {
        return RedirectToAction("Index");
    }
}