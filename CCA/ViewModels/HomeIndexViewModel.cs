using CCA.Models;

namespace CCA.ViewModels;

public class HomeIndexViewModel
{
    public List<TrendingCategoryViewModel> TrendingCategories { get; set; } = new();
    public List<CommissionRequest> RecentCommissions { get; set; } = new();
    public bool ShowRecentCommissions { get; set; }
    public int TotalArtists { get; set; }
    public int TotalCommissions { get; set; }
}

public class TrendingCategoryViewModel
{
    public string Name { get; set; } = string.Empty;
    public int ArtworkCount { get; set; }
    public int ArtistCount { get; set; }
    public string? FeaturedImageUrl { get; set; } // Main artwork image for the card
    public List<Artwork> SampleArtworks { get; set; } = new(); // For grid layout
}

public class CategoryStat
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}