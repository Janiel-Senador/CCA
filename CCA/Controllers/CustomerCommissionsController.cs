using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CCA.Data;
using CCA.Models;

namespace CCA.Controllers;

[Authorize]
public class CustomerCommissionsController : Controller
{
    private readonly CCFADbContext _context = null!;
    private readonly UserManager<ApplicationUser> _userManager = null!;

    public CustomerCommissionsController(CCFADbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ✅ GET: Customer's Commission Requests History
    [HttpGet]
    [Route("customer/commissions")]
    public async Task<IActionResult> Index()
    {
        var customer = await _userManager.GetUserAsync(User);
        if (customer == null) return Challenge();

        var requests = await _context.CommissionRequests
            .Include(cr => cr.Artist)  // Load artist info for display
            .Where(cr => cr.CustomerId == customer.Id)
            .OrderByDescending(cr => cr.CreatedAt)
            .ToListAsync();

        return View(requests);
    }

    // ✅ GET: View single commission details
    [HttpGet]
    [Route("customer/commissions/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var customer = await _userManager.GetUserAsync(User);
        if (customer == null) return Challenge();

        var request = await _context.CommissionRequests
            .Include(cr => cr.Artist)
            .FirstOrDefaultAsync(cr => cr.Id == id && cr.CustomerId == customer.Id);

        if (request == null) return NotFound();

        return View(request);
    }

    // ✅ POST: Cancel a pending commission (customer can cancel before artist accepts)
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("customer/commissions/{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var customer = await _userManager.GetUserAsync(User);
        if (customer == null) return Challenge();

        var request = await _context.CommissionRequests
            .FirstOrDefaultAsync(cr => cr.Id == id && cr.CustomerId == customer.Id);

        if (request == null) return NotFound();
        if (request.Status != "Pending")
        {
            TempData["Error"] = "Only pending commissions can be cancelled.";
            return RedirectToAction("Details", new { id });
        }

        request.Status = "Cancelled";
        request.RespondedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Commission cancelled.";
        return RedirectToAction("Index");
    }
}