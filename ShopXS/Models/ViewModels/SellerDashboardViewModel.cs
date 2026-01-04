namespace ShopXS.Models.ViewModels;

public class SellerDashboardViewModel
{
    public int TotalProducts { get; set; }
    public int ApprovedProducts { get; set; }
    public int PendingProducts { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TodayRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public List<SellerOrderViewModel> RecentOrders { get; set; } = new();
}

public class SellerOrderViewModel
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string ProductTitle { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
