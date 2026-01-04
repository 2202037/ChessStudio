using ShopXS.Models.Entities;
using ShopXS.Models.ViewModels;

namespace ShopXS.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(string userId, CheckoutViewModel model);
    Task<Order?> GetOrderByIdAsync(int id);
    Task<Order?> GetOrderByNumberAsync(string orderNumber);
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<IEnumerable<Order>> GetOrdersBySellerAsync(string sellerId);
    Task<Order> UpdateOrderStatusAsync(int orderId, OrderStatus status);
    Task<int> GetPendingOrdersCountAsync();
    Task<decimal> GetTotalRevenueAsync();
    Task<decimal> GetTodayRevenueAsync();
}
