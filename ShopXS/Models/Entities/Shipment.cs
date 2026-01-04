using System.ComponentModel.DataAnnotations;

namespace ShopXS.Models.Entities;

public enum ShipmentStatus
{
    Processing,
    Shipped,
    InTransit,
    OutForDelivery,
    Delivered,
    Failed
}

public class Shipment
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    [StringLength(100)]
    public string? TrackingNumber { get; set; }

    [StringLength(100)]
    public string? Carrier { get; set; }

    public ShipmentStatus Status { get; set; } = ShipmentStatus.Processing;

    public DateTime? ShippedDate { get; set; }

    public DateTime? EstimatedDelivery { get; set; }

    public DateTime? ActualDelivery { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Order Order { get; set; } = null!;
}
