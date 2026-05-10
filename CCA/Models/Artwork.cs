using System.ComponentModel.DataAnnotations;

namespace CCA.Models;

public class Artwork
{
    public int Id { get; set; }
    public int ArtistId { get; set; }
    public Artist? Artist { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }

    [StringLength(100)]
    public string? Medium { get; set; }

    public int? Year { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}