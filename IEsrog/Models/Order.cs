namespace IEsrog.Models;

public class Order
{
    public string Id { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public List<OrderItem> Items { get; set; } = new();

    public string Comment { get; set; } = string.Empty;

    public bool IsPaid { get; set; }

    public bool IsPacked { get; set; }

    public bool IsShipped { get; set; }

    public DateTime CreatedAt { get; set; }

    public double ShippingCost { get; set; }

    public string? Recipient { get; set; }
}