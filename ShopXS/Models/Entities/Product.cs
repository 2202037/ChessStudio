using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopXS.Models.Entities;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? DiscountPrice { get; set; }

    public int Stock { get; set; }

    [StringLength(100)]
    public string? SKU { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public string? ImageUrls { get; set; } // JSON array of image URLs

    public int CategoryId { get; set; }

    public string SellerId { get; set; } = string.Empty;

    public bool IsApproved { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public bool IsFeatured { get; set; } = false;

    public decimal AverageRating { get; set; } = 0;

    public int ReviewCount { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Category Category { get; set; } = null!;
    public virtual ApplicationUser Seller { get; set; } = null!;
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
}
