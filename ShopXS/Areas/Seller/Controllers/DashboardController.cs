using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopXS.Data;
using ShopXS.Models.Entities;
using ShopXS.Models.ViewModels;
using ShopXS.Services;
using Microsoft.EntityFrameworkCore;

namespace ShopXS.Areas.Seller.Controllers;

[Area("Seller")]
[Authorize(Roles = "Seller")]
public class DashboardController : Controller
{
    private readonly IProductService _productService;
    private readonly IOrderService _orderService;
    private readonly ApplicationDbContext _context;

    public DashboardController(
        IProductService productService,
        IOrderService orderService,
        ApplicationDbContext context)
    {
        _productService = productService;
        _orderService = orderService;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (sellerId == null)
        {
            return RedirectToAction("Login", "Account", new { area = "" });
        }

        var products = await _productService.GetProductsBySellerAsync(sellerId);
        var orders = await _orderService.GetOrdersBySellerAsync(sellerId);
        
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var viewModel = new SellerDashboardViewModel
        {
            TotalProducts = products.Count(),
            ApprovedProducts = products.Count(p => p.IsApproved),
            PendingProducts = products.Count(p => !p.IsApproved),
            TotalOrders = orders.Count(),
            PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Processing),
            TotalRevenue = orders.Where(o => o.Status != OrderStatus.Cancelled)
                .SelectMany(o => o.OrderItems.Where(oi => oi.Product.SellerId == sellerId))
                .Sum(oi => oi.TotalPrice),
            TodayRevenue = orders.Where(o => o.CreatedAt >= today && o.Status != OrderStatus.Cancelled)
                .SelectMany(o => o.OrderItems.Where(oi => oi.Product.SellerId == sellerId))
                .Sum(oi => oi.TotalPrice),
            MonthlyRevenue = orders.Where(o => o.CreatedAt >= monthStart && o.Status != OrderStatus.Cancelled)
                .SelectMany(o => o.OrderItems.Where(oi => oi.Product.SellerId == sellerId))
                .Sum(oi => oi.TotalPrice),
            RecentOrders = orders.Take(10).SelectMany(o => o.OrderItems
                .Where(oi => oi.Product.SellerId == sellerId)
                .Select(oi => new SellerOrderViewModel
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber,
                    ProductTitle = oi.Product.Title,
                    Quantity = oi.Quantity,
                    Total = oi.TotalPrice,
                    Status = o.Status.ToString(),
                    CreatedAt = o.CreatedAt
                })).ToList()
        };

        return View(viewModel);
    }
}
