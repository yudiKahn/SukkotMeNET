namespace IEsrog.Models;

public class Product
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public double Price { get; set; }

    public string PricesType { get; set; } = string.Empty;

    public string[]? Options { get; set; }

    public ExtraOptions[]? ExtraOptions { get; set; }

    public int Group { get; set; }

    public ProductInclude[]? Includes { get; set; }

    public override string ToString() => Name;
}

public class ExtraOptions
{
    public required string Option { get; set; }

    public double Price { get; set; }
}

public class ProductInclude
{
    public string ProductId { get; set; } = string.Empty;
    public int Qty { get; set; }
}