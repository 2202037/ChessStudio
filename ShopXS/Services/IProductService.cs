using ShopXS.Models.Entities;

namespace ShopXS.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<IEnumerable<Product>> GetApprovedProductsAsync();
    Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count);
    Task<IEnumerable<Product>> GetNewProductsAsync(int count);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> GetProductsBySellerAsync(string sellerId);
    Task<IEnumerable<Product>> SearchProductsAsync(string? searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice, string? sortBy, int page, int pageSize);
    Task<int> GetSearchResultsCountAsync(string? searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice);
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
    Task<bool> ApproveProductAsync(int id);
    Task<int> GetPendingApprovalCountAsync();
}
