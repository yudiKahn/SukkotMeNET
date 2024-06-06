namespace SukkotMeNET.Models;

public class Item
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public required string Category { get; set; }

    public double[] Prices { get; set; } = [];

    public string[] PricesTypes { get; set; } = [];

    public string[] Options { get; set; } = [];

    public override string ToString() => Name;
}