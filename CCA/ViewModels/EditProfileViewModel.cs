using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CCA.ViewModels;

public class EditProfileViewModel
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    public IFormFile? PortraitFile { get; set; }
    public string? ExistingPortraitUrl { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    [StringLength(1000)]
    public string? ArtistBio { get; set; }
    public string? ArtistLocation { get; set; }

    [Url]
    public string? WebsiteUrl { get; set; }
    public string? SocialMediaHandle { get; set; }

    public string UserRole { get; set; } = "Customer";
}