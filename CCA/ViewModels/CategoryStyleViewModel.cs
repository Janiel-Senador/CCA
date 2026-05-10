using CCA.Models;

namespace CCA.ViewModels;

public class CategoryStyleViewModel
{
    public string StyleName { get; set; } = string.Empty;
    public List<ArtistWithStyleArtworksViewModel> Artists { get; set; } = new();
}

public class ArtistWithStyleArtworksViewModel
{
    public Artist Artist { get; set; } = null!;
    public List<Artwork> StyleArtworks { get; set; } = new();
}