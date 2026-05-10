using CCA.Models;

namespace CCA.ViewModels;

public class DiscoveryViewModel
{
    public Artist? FeaturedArtist { get; set; }
    public List<TrendingCategory> TrendingCategories { get; set; } = new();
    public List<CommissionSummary> RecentCommissions { get; set; } = new();
}

public class TrendingCategory
{
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int ArtistCount { get; set; }
}

public class CommissionSummary
{
    public string Title { get; set; } = string.Empty;
    public string ArtistName { get; set; } = string.Empty;
    public string ArtistSlug { get; set; } = string.Empty;
    public string? ArtworkImage { get; set; }
    public decimal Price { get; set; }
    public decimal Rating { get; set; }
}