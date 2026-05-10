using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CCA.ViewModels;

public class CommissionRequestViewModel
{
    public int ArtistId { get; set; }

    [Required, StringLength(200)]
    public string ArtistName { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required, Range(1, 10000)]
    public decimal BasePrice { get; set; }

    [Required, Range(1, 10000)]
    public decimal Total { get; set; }

    public string? TurnaroundOption { get; set; } = "standard";

    // ✅ ADD: Dropdown options for commission tiers
    public List<SelectListItem>? AvailableTiers { get; set; }
}