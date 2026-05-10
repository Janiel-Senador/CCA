using System.ComponentModel.DataAnnotations;

namespace CCA.Models;

public class CommissionRequest
{
    public int Id { get; set; }

    public int ArtistId { get; set; }
    public Artist? Artist { get; set; }

    public string CustomerId { get; set; } = string.Empty;
    public ApplicationUser? Customer { get; set; }

    [Required, StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }
    public decimal TotalPrice { get; set; }

    public string TurnaroundOption { get; set; } = "standard";

    // ✅ NEW: Status for artist to manage
    [StringLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Accepted, Declined, Completed, Cancelled

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }
    public string? ArtistResponse { get; set; }
}