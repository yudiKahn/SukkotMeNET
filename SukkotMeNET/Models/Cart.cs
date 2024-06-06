namespace SukkotMeNET.Models;

public class Cart
{
    public string Id { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public List<OrderItem> Items { get; set; }

    public string Comment { get; set; } = string.Empty;

    public Cart()
    {
        Items = new();
    }
}