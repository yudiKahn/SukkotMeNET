using IEsrog.Models;

namespace IEsrog.Services
{
    public class AppStateService
    {
        public User User { get; set; }
        
        public Cart Cart { get; set; }

        public IEnumerable<Product> Products { get; set; }
        public ProductGrpModel? Product { get; set; }

        public List<Order> UserOrders { get; set; }

        public List<Alert> Alerts { get; set; }

        public AdminState AdminState { get; set; }
        public bool IsLoading { get; set; }
        public User? ForUser { get; set; }

        public AppStateService()
        {
            User = new User();
            Cart = new Cart();
            AdminState = new AdminState();
            UserOrders = new List<Order>();
            Alerts = new List<Alert>();
            Products = Array.Empty<Product>();
        }
    }
}
