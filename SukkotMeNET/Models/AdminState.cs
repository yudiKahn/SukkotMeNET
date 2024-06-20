namespace IEsrog.Models;

public class AdminState
{
    public User? CurrentAdminUser { get; set; }
    public List<Order> AllOrders { get; set; }
    public List<User> AllUsers { get; set; }

    public AdminState()
    {
        AllOrders = new();
        AllUsers = new();
    }
}