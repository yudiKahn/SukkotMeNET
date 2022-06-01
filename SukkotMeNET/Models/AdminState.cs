namespace SukkotMeNET.Models
{
    public class AdminState
    {
        public List<Order> AllOrders { get; set; }
        public List<User> AllUsers { get; set; }

        public AdminState()
        {
            AllOrders = new();
            AllUsers = new();
        }
    }
}
