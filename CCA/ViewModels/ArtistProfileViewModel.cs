using CCA.Models;

namespace CCA.ViewModels;

public class ArtistProfileViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? PortraitUrl { get; set; }
    public bool IsVerified { get; set; }
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public decimal Rating { get; set; }
    public string? ResponseTime { get; set; }
    public string? ArtStyles { get; set; }
    public List<Artwork>? Portfolio { get; set; }
    public List<CommissionTier>? Rates { get; set; }

    // ✅ ADDED: Fixes CS1061 error
    public ApplicationUser? ApplicationUser { get; set; }
}