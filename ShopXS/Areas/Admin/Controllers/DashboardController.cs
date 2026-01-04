using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopXS.Data;
using ShopXS.Models.Entities;
using ShopXS.Models.ViewModels;
using ShopXS.Services;
using Microsoft.EntityFrameworkCore;

namespace ShopXS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IProductService _productService;
    private readonly IOrderService _orderService;

    public DashboardController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IProductService productService,
        IOrderService orderService)
    {
        _context = context;
        _userManager = userManager;
        _productService = productService;
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var orders = await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .Include(o => o.User)
            .ToListAsync();

        var viewModel = new AdminDashboardViewModel
        {
            TotalUsers = users.Count(u => !u.IsSeller),
            TotalSellers = users.Count(u => u.IsSeller),
            TotalProducts = await _context.Products.CountAsync(),
            TotalOrders = await _context.Orders.CountAsync(),
            PendingOrders = await _orderService.GetPendingOrdersCountAsync(),
            TotalRevenue = await _orderService.GetTotalRevenueAsync(),
            TodayRevenue = await _orderService.GetTodayRevenueAsync(),
            PendingProductApprovals = await _productService.GetPendingApprovalCountAsync(),
            OpenSupportTickets = await _context.SupportTickets.CountAsync(st => st.Status == TicketStatus.Open),
            RecentOrders = orders.Select(o => new RecentOrderViewModel
            {
                OrderId = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = $"{o.User.FirstName} {o.User.LastName}",
                Total = o.TotalAmount,
                Status = o.Status.ToString(),
                CreatedAt = o.CreatedAt
            }).ToList()
        };

        return View(viewModel);
    }
}
