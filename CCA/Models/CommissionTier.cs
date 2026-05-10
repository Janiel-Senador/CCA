using System.ComponentModel.DataAnnotations.Schema;
namespace CCA.Models;

public class CommissionTier { public int Id { get; set; } public string Name { get; set; } = ""; public string Description { get; set; } = ""; [Column(TypeName = "decimal(18,2)")] public decimal Price { get; set; } public int ArtistId { get; set; } }