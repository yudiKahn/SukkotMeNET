namespace IEsrog.Models;

public class OrderItem
{

    public string Id { get; set; } = string.Empty;

    public required string ProductId { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public double Price { get; set; }

    public string? PriceType { get; set; }

    public string? Option { get; set; }
        
    public int Qty { get; set; }

    public bool ByAdmin { get; set; }

    public override string ToString() => $"{Name} {PriceType} {Option}";

    public OrderItem Clone()
    {
        return new OrderItem
        {
            Id = Id,
            ProductId = ProductId,
            Category = Category,
            Name = Name,
            Price = Price,
            PriceType = PriceType,
            Option = Option,
            Qty = Qty,
            ByAdmin = ByAdmin
        };
    }
}