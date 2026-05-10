using System.ComponentModel.DataAnnotations;

namespace CCA.Models;

public class Message
{
    public int Id { get; set; }

    [Required]
    public string SenderId { get; set; } = string.Empty;
    public ApplicationUser? Sender { get; set; } // ✅ Navigation to sender

    [Required]
    public string ReceiverId { get; set; } = string.Empty;
    public ApplicationUser? Receiver { get; set; } // ✅ Navigation to receiver

    [Required, StringLength(2000)]
    public string Content { get; set; } = string.Empty;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;

    public int? CommissionRequestId { get; set; }
    public CommissionRequest? CommissionRequest { get; set; }
}