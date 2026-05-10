using CCA.Models;

namespace CCA.ViewModels;

public class ConversationViewModel
{
    public ApplicationUser Partner { get; set; } = null!;
    public Message LastMessage { get; set; } = null!;
    public int UnreadCount { get; set; }

    // ✅ Helper property for null-safe profile picture URL
    public string PartnerPortraitUrl =>
        Partner.PortraitUrl?.StartsWith("/") == true
            ? Partner.PortraitUrl
            : "/" + (Partner.PortraitUrl ?? $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(Partner.FullName ?? "User")}&background=random");
}