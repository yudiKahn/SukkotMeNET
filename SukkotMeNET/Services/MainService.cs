using SukkotMeNET.Extensions;
using SukkotMeNET.Interfaces;
using SukkotMeNET.Models;

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
            if(_AppState.Cart?.Items == null)
            {
                _AppState.Cart = new Cart();
            }
            if(item == null || item.Qty < 1)
                return;

            var cart = _AppState.Cart;
            cart.Items.AddOrMerge(item);
            await _Repository.CartsRepository.UpdateFirstAsync(c => c.UserId == _AppState.User.Id, cart);
        }

        public async Task<User?> LoginAsync(User user)
        {
            string email = user.Email;
            string password = user.Password;

            var users = await _Repository.UsersRepository.ReadAllAsync(u => u.Email == email);
            var user1 = users.FirstOrDefault(u => BCrypt.Net.BCrypt.Verify(password, u?.Password));

            _AppState.User = user1;

            if (user1 != null)
            {
                StateHasChanged?.Invoke(this, EventArgs.Empty);

                InitUserCart();
                InitUserOrders();
                if (user1.IsAdmin)
                    InitAdminState();
            }

            return user1;
        }

        public async Task<bool> LoginUserFromLocalStorage(string token)
        {
            var user = await _Repository.UsersRepository.ReadFirstAsync(u => u.Id == token);
            if(user != null)
            {
                _AppState.User = user;
                StateHasChanged?.Invoke(this, EventArgs.Empty);

                InitUserCart();
                InitUserOrders();
                if (user.IsAdmin)
                    InitAdminState();
                return true;
            }
            return false;
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

        public async void RemoveItemFromCart(OrderItem item)
        {
            _AppState.Cart.Items.Remove(item);
            await _Repository.CartsRepository.UpdateFirstAsync(c => c.Id == _AppState.Cart.Id, _AppState.Cart);

        }

        public async Task<bool> CreateOrderFromCart()
        {
            if (_AppState?.Cart is null || _AppState.User is null)
                return await Task.FromResult(false);

            try
            {
                var order = new Order();
                order.Items = _AppState.Cart.Items;
                order.UserId = _AppState.User.Id;
                order.CreatedAt = DateTime.Now;
                await _Repository.OrdersRepository.WriteAsync(order);

                await _Repository.CartsRepository.DeleteFirstAsync(c => c.UserId == _AppState.User.Id && c.Id == _AppState.Cart.Id);
                _AppState.Cart = null;
                return true;
            }
            catch
            {
                return false;
            }
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
            return Task.CompletedTask;
        }
    }
}
