using MongoDB.Bson;
using SukkotMeNET.Models;

namespace SukkotMeNET.Services
{
    public class AppStateService
    {
        public User? User { get; set; }

        public Cart? Cart { get; set; }

        public IEnumerable<Item> ShopItems { get; set; }

        public List<Order>? UserOrders { get; set; }

        public List<Alert> Alerts { get; set; }

        public AdminState AdminState { get; set; }

        public AppStateService()
        {
            AdminState = new AdminState();
            UserOrders = new List<Order>();
            Alerts = new List<Alert>();
            ShopItems = Array.Empty<Item>();
        }
    }
}
