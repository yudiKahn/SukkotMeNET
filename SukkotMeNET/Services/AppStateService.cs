using MongoDB.Bson;
using SukkotMeNET.Models;

namespace SukkotMeNET.Services
{
    public class AppStateService
    {
        public User User { get; set; }

        public Cart Cart { get; set; }

        public List<Order> Orders { get; set; }

        public List<Alert> Alerts { get; set; }

        public AppStateService()
        {
            Orders = new List<Order>();
            Alerts = new List<Alert>();
        }
    }
}
