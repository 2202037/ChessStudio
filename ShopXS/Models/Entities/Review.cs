using System.ComponentModel.DataAnnotations;

namespace ShopXS.Models.Entities;

public class Review
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string UserId { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(100)]
    public string? Title { get; set; }

    [StringLength(2000)]
    public string? Comment { get; set; }

    public bool IsApproved { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Product Product { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}
