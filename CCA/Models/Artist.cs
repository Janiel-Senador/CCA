using System.ComponentModel.DataAnnotations;

namespace CCA.Models;

public class Artist
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [Required] public string Slug { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string PortraitUrl { get; set; } = "https://via.placeholder.com/400";
    public decimal Rating { get; set; } = 5.0m;
    public DateTime MemberSince { get; set; } = DateTime.Now;
    public string ResponseTime { get; set; } = "Under 24 hours";
    public bool IsVerified { get; set; }
    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser? ApplicationUser { get; set; }
    public ICollection<CommissionRequest> CommissionRequests { get; set; } = new List<CommissionRequest>();
    public ICollection<Artwork> Artworks { get; set; } = new List<Artwork>();
    public ICollection<CommissionTier> CommissionTiers { get; set; } = new List<CommissionTier>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    // e.g., "Abstract Art, Realism, Pop Art"
    public string? ArtStyles { get; set; }
}