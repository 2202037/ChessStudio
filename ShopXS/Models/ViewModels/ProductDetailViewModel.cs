using ShopXS.Models.Entities;

namespace ShopXS.Models.ViewModels;

public class ProductDetailViewModel
{
    public Product Product { get; set; } = null!;
    public List<Review> Reviews { get; set; } = new();
    public List<Product> RelatedProducts { get; set; } = new();
    public bool IsInWishlist { get; set; }
    public bool CanReview { get; set; }
}
