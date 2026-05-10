using CCA.Models;

namespace CCA.ViewModels;

public class ArtworksByCategoryViewModel
{
    public string Category { get; set; } = string.Empty;
    public List<Artwork> Artworks { get; set; } = new();
    public int ArtistCount { get; set; }
}