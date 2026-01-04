using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopXS.Models.Entities;
using ShopXS.Services;

namespace ShopXS.Areas.Seller.Controllers;

[Area("Seller")]
[Authorize(Roles = "Seller")]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (sellerId == null)
        {
            return RedirectToAction("Login", "Account", new { area = "" });
        }

        var orders = await _orderService.GetOrdersBySellerAsync(sellerId);
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var order = await _orderService.GetOrderByIdAsync(id);
        
        if (order == null)
        {
            return NotFound();
        }

        // Check if this order contains any products from the current seller
        var sellerItems = order.OrderItems.Where(oi => oi.Product.SellerId == sellerId).ToList();
        if (!sellerItems.Any())
        {
            return Forbid();
        }

        ViewBag.SellerItems = sellerItems;
        return View(order);
    }
}
