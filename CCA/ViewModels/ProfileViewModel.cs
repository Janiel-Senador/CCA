namespace CCA.ViewModels;

public class ProfileViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? PortraitUrl { get; set; }
    public string? Bio { get; set; }
    public string? ArtistBio { get; set; }
    public string? ArtistLocation { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? SocialMediaHandle { get; set; }
    public DateTime MemberSince { get; set; }

    // Artist stats
    public decimal ArtistRating { get; set; }
    public int TotalCommissions { get; set; }
    public int ActiveCommissions { get; set; }
}