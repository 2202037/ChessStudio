using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopXS.Models.ViewModels;
using ShopXS.Services;

namespace ShopXS.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;

    public OrderController(IOrderService orderService, ICartService cartService)
    {
        _orderService = orderService;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var orders = await _orderService.GetOrdersByUserIdAsync(userId);
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (order.UserId != userId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        return View(order);
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var cartViewModel = await _cartService.GetCartViewModelAsync(userId);
        if (!cartViewModel.Items.Any())
        {
            TempData["ErrorMessage"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        var model = new CheckoutViewModel
        {
            SubTotal = cartViewModel.SubTotal,
            ShippingCost = cartViewModel.ShippingCost,
            Tax = cartViewModel.Tax,
            Total = cartViewModel.Total
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Recalculate totals
        var cartViewModel = await _cartService.GetCartViewModelAsync(userId);
        model.SubTotal = cartViewModel.SubTotal;
        model.ShippingCost = cartViewModel.ShippingCost;
        model.Tax = cartViewModel.Tax;
        model.Total = cartViewModel.Total;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!cartViewModel.Items.Any())
        {
            TempData["ErrorMessage"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        var order = await _orderService.CreateOrderAsync(userId, model);

        return RedirectToAction("Confirmation", new { id = order.Id });
    }

    public async Task<IActionResult> Confirmation(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (order.UserId != userId)
        {
            return Forbid();
        }

        return View(order);
    }

    public async Task<IActionResult> Track(string orderNumber)
    {
        if (string.IsNullOrEmpty(orderNumber))
        {
            return View();
        }

        var order = await _orderService.GetOrderByNumberAsync(orderNumber);
        if (order == null)
        {
            ViewBag.ErrorMessage = "Order not found.";
            return View();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (order.UserId != userId && !User.IsInRole("Admin"))
        {
            ViewBag.ErrorMessage = "Order not found.";
            return View();
        }

        return View(order);
    }
}
