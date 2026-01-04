using Microsoft.EntityFrameworkCore;
using ShopXS.Data;
using ShopXS.Models.Entities;
using ShopXS.Models.ViewModels;

namespace ShopXS.Services;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;
    private const decimal TAX_RATE = 0.10m; // 10% tax
    private const decimal SHIPPING_COST = 9.99m;

    public CartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetCartByUserIdAsync(string userId)
    {
        return await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<CartViewModel> GetCartViewModelAsync(string userId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        var cartItems = await _context.CartItems
            .Include(ci => ci.Product)
            .Where(ci => ci.CartId == cart.Id)
            .ToListAsync();

        var items = cartItems.Select(ci => new CartItemViewModel
        {
            CartItemId = ci.Id,
            ProductId = ci.ProductId,
            ProductTitle = ci.Product.Title,
            ProductImage = ci.Product.ImageUrl,
            UnitPrice = ci.Product.DiscountPrice ?? ci.Product.Price,
            Quantity = ci.Quantity,
            TotalPrice = (ci.Product.DiscountPrice ?? ci.Product.Price) * ci.Quantity,
            MaxQuantity = ci.Product.Stock
        }).ToList();

        var subTotal = items.Sum(i => i.TotalPrice);
        var tax = subTotal * TAX_RATE;
        var shippingCost = items.Any() ? SHIPPING_COST : 0;

        return new CartViewModel
        {
            Cart = cart,
            Items = items,
            SubTotal = subTotal,
            ShippingCost = shippingCost,
            Tax = tax,
            Total = subTotal + tax + shippingCost
        };
    }

    public async Task<Cart> GetOrCreateCartAsync(string userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        return cart;
    }

    public async Task AddToCartAsync(string userId, int productId, int quantity)
    {
        var cart = await GetOrCreateCartAsync(userId);
        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = quantity
            };
            _context.CartItems.Add(cartItem);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCartItemQuantityAsync(int cartItemId, int quantity)
    {
        var cartItem = await _context.CartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

        if (cartItem != null)
        {
            if (quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
            }
            cartItem.Cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveFromCartAsync(int cartItemId)
    {
        var cartItem = await _context.CartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            cartItem.Cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearCartAsync(string userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart != null)
        {
            _context.CartItems.RemoveRange(cart.CartItems);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetCartItemCountAsync(string userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        return cart?.CartItems.Sum(ci => ci.Quantity) ?? 0;
    }
}
