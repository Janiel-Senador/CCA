namespace CCA.ViewModels;

public class ArtistDashboardViewModel
{
    public string ArtistName { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public decimal TotalEarnings { get; set; }
    public int PendingRequests { get; set; }
    public int ActiveCommissions { get; set; }
    public int PortfolioCount { get; set; }
    public List<CommissionSummaryViewModel> RecentCommissions { get; set; } = new();
}

