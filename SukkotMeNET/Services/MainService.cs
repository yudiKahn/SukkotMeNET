using MongoDB.Bson;
using SukkotMeNET.Extensions;
using SukkotMeNET.Interfaces;
using SukkotMeNET.Models;

namespace SukkotMeNET.Services
{
    public class MainService : BackgroundService
    {
        readonly AppStateService _AppState;
        readonly IRepositoryService _Repository;
        private readonly EmailService _EmailService;

        public event EventHandler StateHasChanged;

        public MainService(
            AppStateService appState,
            IRepositoryService repositoryService,
            EmailService emailService)
        {
            _AppState = appState;
            _Repository = repositoryService;
            _EmailService = emailService;
        }

        //Cart
        public async void AddItemToCart(OrderItem item)
        {
            try
            {
                if (item == null)
                    throw new Exception("Unknown error (0x1), Please contact developer");
                if (item.Qty < 1)
                    throw new Exception("Quantity cannot be less then one");

                if (_AppState.Cart == null)
                {
                    await Task.Run(InitUserCart);
                }
                var cart = _AppState.Cart;

                cart.Items.AddOrMerge(item);

                await _Repository.CartsRepository.UpdateFirstAsync(c => c.UserId == _AppState.User.Id, cart);
                
                _AppState.Cart = cart;
                StateHasChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                AddAlert(new Alert("Eroor", e.Message, AlertType.Error));
            }
        }

        public async void RemoveItemFromCart(OrderItem item)
        {
            _AppState.Cart.Items.Remove(item);
            await _Repository.CartsRepository.UpdateFirstAsync(c => c.Id == _AppState.Cart.Id, _AppState.Cart);
            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        public async void SaveCurrentCart()
        {
            if (_AppState.Cart == null || _AppState.User == null)
                return;
            await _Repository.CartsRepository.UpdateFirstAsync(c => c.UserId == _AppState.User.Id, _AppState.Cart);
        }

        //Login
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
            if (_AppState.User is null)
            {
                var user = await _Repository.UsersRepository.ReadFirstAsync(u => u.Id == token);
                if (user != null)
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
            return true;
        }

        public void Logout()
        {
            _AppState.User = null;
            _AppState.Cart = null;
            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        //Alerts
        public void AddAlert(Alert alert)
        {
            _AppState.Alerts.Add(alert);
            StateHasChanged?.Invoke(this, EventArgs.Empty);
            Task.Delay(4000).ContinueWith(t => RemoveAlert(alert));
        }

        public void RemoveAlert(Alert alert)
        {
            _AppState.Alerts.Remove(alert);
            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        //Order
        public async Task<bool> CreateOrderFromCart()
        {
            if (_AppState?.Cart is null || _AppState.User is null)
                return await Task.FromResult(false);

            try
            {
                var order = new Order
                {
                    Items = _AppState.Cart.Items.GetWithSaleItems(),
                    UserId = _AppState.User.Id,
                    CreatedAt = DateTime.Now
                };
                await _Repository.OrdersRepository.WriteAsync(order);

                await _Repository.CartsRepository.DeleteFirstAsync(c => c.UserId == _AppState.User.Id && c.Id == _AppState.Cart.Id);
                _AppState.Cart = null;

                var invoice = InvoiceHelper.GetInvoiceHTML(order, _AppState.User);
                await _EmailService.SendAsync(_AppState.User.Email, "Order Invoice", invoice);
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
            if (cart != null)
            {
                _AppState.Cart = cart;
            }
            else
            {
                _AppState.Cart = new Cart();
                _AppState.Cart.Id = ObjectId.GenerateNewId().ToString();
                _AppState.Cart.UserId = _AppState.User.Id;
            }

            StateHasChanged.Invoke(this, EventArgs.Empty);
        }

        private async void InitUserOrders()
        {
            if (_AppState.User == null)
                return;

            var userId = _AppState.User.Id;
            var orders = await _Repository.OrdersRepository.ReadAllAsync(o => o.UserId == userId);
            _AppState.UserOrders = orders?.ToList();

            StateHasChanged.Invoke(this, EventArgs.Empty);
        }

        private async void InitAdminState()
        {
            var orders = await _Repository.OrdersRepository.ReadAllAsync();
            _AppState.AdminState.AllOrders = orders.ToList();

            var users = await _Repository.UsersRepository.ReadAllAsync();
            _AppState.AdminState.AllUsers = users.ToList();

            StateHasChanged.Invoke(this, EventArgs.Empty);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //init shop items
            _AppState.ShopItems = await _Repository.ItemsRepository.ReadAllAsync() ?? Array.Empty<Item>();
            Console.WriteLine($"items: {_AppState.ShopItems.Count()}");
        }
    }
}
