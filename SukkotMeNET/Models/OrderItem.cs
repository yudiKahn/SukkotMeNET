namespace SukkotMeNET.Models;

public class OrderItem
{

    public string Id { get; set; } = string.Empty;
    
    public string ProductId { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public double Price { get; set; }

    public string? PriceType { get; set; }

    public string? Option { get; set; }
        
    public int Qty { get; set; }

    public bool ByAdmin { get; set; }

    public override string ToString() => $"{Name} {PriceType} {Option}";
}