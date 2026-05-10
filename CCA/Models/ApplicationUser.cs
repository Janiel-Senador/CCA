using Microsoft.AspNetCore.Identity;

namespace CCA.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    // ✅ Roles: "Customer", "Artist", "Admin"
    public string UserRole { get; set; } = "Customer";

    // Artist-specific fields
    public string? ArtistBio { get; set; }
    public string? ArtistLocation { get; set; }
    public string? PortraitUrl { get; set; }
    public bool IsVerified { get; set; } = false;
    public DateTime? VerifiedAt { get; set; }

    // Profile editing
    public string? Bio { get; set; } // For customers
    public string? WebsiteUrl { get; set; }
    public string? SocialMediaHandle { get; set; }

    // Navigation
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    public Artist? ArtistProfile { get; set; }
}