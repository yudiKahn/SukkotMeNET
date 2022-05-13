using SukkotMeNET.Interfaces;
using SukkotMeNET.Models;
using System.Linq.Expressions;

namespace SukkotMeNET.Services
{
    public class MainService : BackgroundService
    {
        readonly AppStateService _AppState;
        readonly IRepositoryService _Repository;

        public event EventHandler StateHasChanged;

        public MainService(
            AppStateService appState,
            IRepositoryService repositoryService)
        {
            _AppState = appState;
            _Repository = repositoryService;
        }

        public async void AddItemToCart(OrderItem item) 
        { 
            if(item == null || item.Qty < 1 || _AppState?.Cart?.Items == null)
                return;

            var cart = _AppState.Cart;
            cart.Items.Add(item);
            await _Repository.CartsRepository.UpdateFirstAsync(c => c.UserId == _AppState.User.Id, cart);
        }

        public async Task<bool> LoginAsync(User user)
        {
            string email = user.Email;
            string password = user.Password;

            var users = await _Repository.UsersRepository.ReadAllAsync(u => u.Email == email);
            var user1 = users.FirstOrDefault(u => BCrypt.Net.BCrypt.Verify(password, u?.Password));

            if(user1 == null)
                return false;

            _AppState.User = user1;
            StateHasChanged?.Invoke(this, EventArgs.Empty);

            InitUserCart();
            InitUserOrders();
            if (user1.IsAdmin)
                InitAdminState();

            return true;
        }

        public void Logout()
        {
            _AppState.User = null;
            _AppState.Cart = null;
            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AddAlert(Alert alert)
        {
            _AppState.Alerts.Add(alert);
            StateHasChanged?.Invoke(this, EventArgs.Empty);
            Task.Delay(2000).ContinueWith(t => RemoveAlert(alert));
        }

        public void RemoveAlert(Alert alert)
        {
            _AppState.Alerts.Remove(alert);
            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        private async void InitUserCart()
        {
            if (_AppState.User == null)
                return;

            var userId = _AppState.User.Id;
            var cart = await _Repository.CartsRepository.ReadFirstAsync(c => c.UserId == userId);
            _AppState.Cart = cart;
        }

        private async void InitUserOrders()
        {

            if (_AppState.User == null)
                return;

            var userId = _AppState.User.Id;
            var orders = await _Repository.OrdersRepository.ReadAllAsync(o => o.UserId == userId);
            _AppState.UserOrders = orders?.ToList();
        }

        private async void InitAdminState()
        {
            var orders = await _Repository.OrdersRepository.ReadAllAsync();
            _AppState.AdminState.AllOrders = orders.ToList();

            var users = await _Repository.UsersRepository.ReadAllAsync();
            _AppState.AdminState.AllUsers = users.ToList();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //TODO: init cached user,
            return Task.CompletedTask;
        }
    }
}
