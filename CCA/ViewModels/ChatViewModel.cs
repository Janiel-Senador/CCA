using CCA.Models;

namespace CCA.ViewModels;

public class ChatViewModel
{
    public ApplicationUser OtherUser { get; set; } = null!;
    public List<Message> Messages { get; set; } = new();
    public string CurrentUserId { get; set; } = string.Empty;
}