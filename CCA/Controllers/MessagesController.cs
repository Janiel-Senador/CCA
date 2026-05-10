using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CCA.Data;
using CCA.Models;
using CCA.ViewModels;

namespace CCA.Controllers;

[Authorize]
public class MessagesController : Controller
{
    private readonly CCFADbContext _context = null!;
    private readonly UserManager<ApplicationUser> _userManager = null!;

    public MessagesController(CCFADbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ✅ GET: List of conversations - Returns List<ConversationViewModel>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        // Get all messages where user is sender or receiver
        var messages = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.SenderId == user.Id || m.ReceiverId == user.Id)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

        // Group by conversation partner and build view models
        var conversations = messages
            .GroupBy(m => m.SenderId == user.Id ? m.Receiver : m.Sender)
            .Select(g =>
            {
                var partner = g.Key;
                var lastMessage = g.OrderByDescending(m => m.SentAt).First();
                var unreadCount = g.Count(m => m.ReceiverId == user.Id && !m.IsRead);

                return new ConversationViewModel
                {
                    Partner = partner!,
                    LastMessage = lastMessage,
                    UnreadCount = unreadCount
                };
            })
            .ToList();

        return View(conversations);
    }

    // ✅ GET: Chat window
    [HttpGet]
    [Route("Messages/Chat")]
    public async Task<IActionResult> Chat(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return Challenge();

        var otherUser = await _userManager.FindByIdAsync(userId);
        if (otherUser == null) return NotFound();

        // Get conversation history
        var messages = await _context.Messages
            .Where(m =>
                (m.SenderId == currentUser.Id && m.ReceiverId == userId) ||
                (m.SenderId == userId && m.ReceiverId == currentUser.Id))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        // Mark messages as read
        var unread = messages.Where(m => m.ReceiverId == currentUser.Id && !m.IsRead);
        foreach (var m in unread) m.IsRead = true;
        await _context.SaveChangesAsync();

        var viewModel = new ChatViewModel
        {
            OtherUser = otherUser,
            Messages = messages,
            CurrentUserId = currentUser.Id
        };

        return View(viewModel);
    }

    // ✅ POST: Send a message
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Messages/Send")]
    public async Task<IActionResult> Send(string receiverId, string content)
    {
        if (string.IsNullOrEmpty(receiverId) || string.IsNullOrWhiteSpace(content))
            return BadRequest();

        var sender = await _userManager.GetUserAsync(User);
        if (sender == null) return Challenge();

        var receiver = await _userManager.FindByIdAsync(receiverId);
        if (receiver == null) return NotFound();

        var message = new Message
        {
            SenderId = sender.Id,
            ReceiverId = receiverId,
            Content = content.Trim(),
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return RedirectToAction("Chat", new { userId = receiverId });
    }
}