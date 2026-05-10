namespace CCA.ViewModels;

public class ArtistSettingsViewModel
{
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string? ResponseTime { get; set; }
    public List<string> SelectedStyles { get; set; } = new();
    public IEnumerable<string> AllStyles { get; set; } = new List<string>();
}