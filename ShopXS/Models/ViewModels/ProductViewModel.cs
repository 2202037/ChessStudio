using System.ComponentModel.DataAnnotations;

namespace ShopXS.Models.ViewModels;

public class ProductViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99")]
    public decimal Price { get; set; }

    [Range(0, 999999.99, ErrorMessage = "Discount price must be between 0 and 999999.99")]
    [Display(Name = "Discount Price")]
    public decimal? DiscountPrice { get; set; }

    [Required(ErrorMessage = "Stock is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock must be a positive number")]
    public int Stock { get; set; }

    [StringLength(100)]
    public string? SKU { get; set; }

    [StringLength(500)]
    [Display(Name = "Image URL")]
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Display(Name = "Featured Product")]
    public bool IsFeatured { get; set; }
}
