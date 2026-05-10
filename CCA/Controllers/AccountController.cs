using CCA.ViewModels;
using CCA.Data;
using CCA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace CCA.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager = null!;
    private readonly SignInManager<ApplicationUser> _signInManager = null!;
    private readonly CCFADbContext _context = null!;
    private readonly IWebHostEnvironment _environment = null!;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        CCFADbContext context,
        IWebHostEnvironment environment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _environment = environment;
    }

    // ✅ GET: Register page
    [HttpGet]
    public IActionResult Register() => View();

    // ✅ POST: Handle registration
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName ?? "User",
            UserRole = model.UserRole,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            // If Artist, create Artist profile
            if (model.UserRole == "Artist")
            {
                var artist = new Artist
                {
                    ApplicationUserId = user.Id,
                    Name = model.FullName ?? "Unknown Artist",
                    Slug = $"{(model.FullName ?? "user").ToLower().Replace(" ", "-")}-{Guid.NewGuid().ToString("N")[..8]}",
                    Bio = model.ArtistBio ?? "New artist on CCA",
                    Location = model.ArtistLocation ?? "Unknown",
                    IsVerified = false,
                    Rating = 5.0m,
                    MemberSince = DateTime.UtcNow,
                    ResponseTime = "Under 48h"
                };
                _context.Artists.Add(artist);
                await _context.SaveChangesAsync();
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    // ✅ GET: Login page
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // ✅ POST: Handle login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
            return RedirectToLocal(returnUrl);
        if (result.IsLockedOut)
            ModelState.AddModelError(string.Empty, "Account locked. Try again later.");
        else
            ModelState.AddModelError(string.Empty, "Invalid email or password.");

        return View(model);
    }

    // ✅ POST: Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // ✅ GET: Access denied page
    [HttpGet]
    public IActionResult AccessDenied() => View();

    // ✅ GET: User profile page
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login");

        var viewModel = new ProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            UserRole = user.UserRole,
            IsVerified = user.IsVerified,
            VerifiedAt = user.VerifiedAt,
            PortraitUrl = user.PortraitUrl,
            Bio = user.Bio,
            ArtistBio = user.ArtistBio,
            ArtistLocation = user.ArtistLocation,
            WebsiteUrl = user.WebsiteUrl,
            SocialMediaHandle = user.SocialMediaHandle,
            MemberSince = DateTime.UtcNow
        };

        if (user.UserRole == "Artist")
        {
            var artist = await _context.Artists
                .Include(a => a.CommissionTiers)
                .Include(a => a.Reviews)
                .FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id);

            if (artist != null)
            {
                viewModel.ArtistRating = artist.Rating;
                viewModel.TotalCommissions = await _context.CommissionRequests
                    .CountAsync(cr => cr.ArtistId == artist.Id);
                viewModel.ActiveCommissions = await _context.CommissionRequests
                    .CountAsync(cr => cr.ArtistId == artist.Id && cr.Status == "In Progress");
            }
        }

        return View(viewModel);
    }

    // ✅ GET: Edit profile page
    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        return View(new EditProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            ExistingPortraitUrl = user.PortraitUrl,
            Bio = user.Bio,
            ArtistBio = user.ArtistBio,
            ArtistLocation = user.ArtistLocation,
            WebsiteUrl = user.WebsiteUrl,
            SocialMediaHandle = user.SocialMediaHandle,
            UserRole = user.UserRole
        });
    }

    // ✅ POST: Handle profile edit - FIX: Profile picture upload working
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        // 🔹 FIX: Profile Picture Upload - Ensure it saves correctly
        if (model.PortraitFile != null && model.PortraitFile.Length > 0)
        {
            // Validate file size (2MB max)
            if (model.PortraitFile.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("PortraitFile", "File size must be under 2MB.");
                return View(model);
            }

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
            var contentType = model.PortraitFile.ContentType?.ToLowerInvariant() ?? string.Empty;
            if (!allowedTypes.Contains(contentType))
            {
                ModelState.AddModelError("PortraitFile", "Only JPG, PNG, or GIF files are allowed.");
                return View(model);
            }

            // Resolve safe web root path
            var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRoot, "uploads", "profiles");
            Directory.CreateDirectory(uploadsFolder);

            // Generate unique filename
            var extension = Path.GetExtension(model.PortraitFile.FileName ?? ".jpg");
            var uniqueFileName = $"{user.Id}_{Guid.NewGuid().ToString("N")[..8]}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file to disk
            using var stream = new FileStream(filePath, FileMode.Create);
            await model.PortraitFile.CopyToAsync(stream);

            // ✅ CRITICAL: Save the URL with leading slash for browser access
            user.PortraitUrl = $"/uploads/profiles/{uniqueFileName}";
        }
        else if (!string.IsNullOrEmpty(model.ExistingPortraitUrl))
        {
            // Keep existing image if no new upload
            user.PortraitUrl = model.ExistingPortraitUrl;
        }
        // else: If no file and no existing URL, leave PortraitUrl as-is (don't clear it)

        // 🔹 Update Profile Fields (Null-Safe Assignments)
        user.FullName = model.FullName ?? user.FullName;
        user.Bio = model.Bio ?? user.Bio;
        user.WebsiteUrl = model.WebsiteUrl ?? user.WebsiteUrl;
        user.SocialMediaHandle = model.SocialMediaHandle ?? user.SocialMediaHandle;

        if (user.UserRole == "Artist")
        {
            user.ArtistBio = model.ArtistBio ?? user.ArtistBio;
            user.ArtistLocation = model.ArtistLocation ?? user.ArtistLocation;

            var artist = await _context.Artists.FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id);
            if (artist != null)
            {
                artist.Bio = model.ArtistBio ?? artist.Bio;
                artist.Location = model.ArtistLocation ?? artist.Location;
                _context.Artists.Update(artist);
            }
        }

        // 🔹 Handle Email Change
        if (model.Email != user.Email)
        {
            var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
            if (!setEmailResult.Succeeded)
                foreach (var error in setEmailResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
        }

        // 🔹 Save User to Database
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            foreach (var error in updateResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

        if (!ModelState.IsValid) return View(model);

        // 🔹 Save any pending changes to Artist entity
        await _context.SaveChangesAsync();

        TempData["Success"] = "Profile updated.";
        return RedirectToAction("Profile");
    }

    // ✅ Helper: Redirect to local URL or home
    private IActionResult RedirectToLocal(string? returnUrl) =>
        Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
}