using System.ComponentModel.DataAnnotations;

namespace HybridLibrarySystem.Core.Model;

public class Reservation
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int BookId { get; set; }
    public Book? Book { get; set; }
    [Required]
    public string UserId { get; set; } = null!;
    public ApplicationUser? User { get; set; }
    public DateTime ReservedAt { get; set; }
}
