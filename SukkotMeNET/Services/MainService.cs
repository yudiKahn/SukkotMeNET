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
            try
            {
                if (_AppState.Cart?.Items == null)
                    _AppState.Cart = new Cart();

                if (item == null)
                    throw new Exception("Unknown error (0x1), Please contact developer");
                if (item.Qty < 1)
                    throw new Exception("Please insert item quantity");


                var cart = _AppState.Cart;
                cart.Items.AddOrMerge(item);
                cart.Items.AddOrMergeRange(SaleItemsToAdd(item).ToArray());

                await _Repository.CartsRepository.UpdateFirstAsync(c => c.UserId == _AppState.User.Id, cart);
            }
            catch (Exception e)
            {
                AddAlert(new Alert("Eroor", e.Message, AlertType.Error));
            }
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
            Task.Delay(4000).ContinueWith(t => RemoveAlert(alert));
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

        private IEnumerable<OrderItem> SaleItemsToAdd(OrderItem newItem)
        {
            List<OrderItem> itemsToAdd = new List<OrderItem>();

            //add 20% extra for israeli sets
            if (newItem.Id == Constants.General.IsraeliSetItemId)
            {
                var qtyToAdd = (int)(0.2 * newItem.Qty);
                itemsToAdd.Add(new OrderItem()
                {
                    Id = newItem.Id,
                    ByAdmin = true,
                    Category = newItem.Category,
                    Name = newItem.Name,
                    Option = newItem.Option,
                    Price = 0,
                    PriceType = newItem.PriceType,
                    Qty = qtyToAdd
                });
            }

            return itemsToAdd;
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //init shop items
            _AppState.ShopItems = await _Repository.ItemsRepository.ReadAllAsync() ?? Array.Empty<Item>();
        }
    }
}
