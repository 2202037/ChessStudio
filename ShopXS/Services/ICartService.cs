using ShopXS.Models.Entities;
using ShopXS.Models.ViewModels;

namespace ShopXS.Services;

public interface ICartService
{
    Task<Cart?> GetCartByUserIdAsync(string userId);
    Task<CartViewModel> GetCartViewModelAsync(string userId);
    Task<Cart> GetOrCreateCartAsync(string userId);
    Task AddToCartAsync(string userId, int productId, int quantity);
    Task UpdateCartItemQuantityAsync(int cartItemId, int quantity);
    Task RemoveFromCartAsync(int cartItemId);
    Task ClearCartAsync(string userId);
    Task<int> GetCartItemCountAsync(string userId);
}
