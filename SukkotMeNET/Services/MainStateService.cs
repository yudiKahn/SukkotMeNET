using MongoDB.Bson;
using SukkotMeNET.Extensions;
using SukkotMeNET.Interfaces;
using SukkotMeNET.Models;

namespace SukkotMeNET.Services
{
    public class MainStateService
    {
        readonly AppStateService _AppState;
        readonly IRepositoryService _Repository;
        readonly EmailService _EmailService;
        readonly InvoiceService _InvoiceService;

        public event EventHandler? StateHasChanged;

        public MainStateService(
            AppStateService appState,
            IRepositoryService repositoryService,
            EmailService emailService,
            InvoiceService invoiceService)
        {
            _AppState = appState;
            _Repository = repositoryService;
            _EmailService = emailService;
            _InvoiceService = invoiceService;
        }

        //Cart
        public async Task<bool> AddItemToCart(OrderItem item)
        {
            try
            {
                var itemClone = item.Clone();

                if (itemClone == null)
                    throw new Exception("Unknown error (0x1), Please contact developer");
                if (itemClone.Qty < 1)
                    throw new Exception("Quantity cannot be less then one");

                /*if (_AppState.Cart == null)
                {
                    Console.WriteLine("Cart is null");
                    _ = await InitUserCart();
                }*/

                _AppState.Cart.Items.AddOrMerge(itemClone);

                await _Repository.CartsRepository.UpdateFirstAsync(c => c.UserId == _AppState.User.Id, _AppState.Cart, false);

                StateHasChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch (Exception e)
            {
                AddAlert(new Alert("Eroor", e.Message, AlertType.Error));
                return false;
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
            /*if (_AppState.Cart is null || _AppState.User == null)
                return;*/
            await _Repository.CartsRepository.UpdateFirstAsync(c => c.UserId == _AppState.User.Id, _AppState.Cart);
        }

        //Login
        public async Task<User?> LoginAsync(User user)
        {
            var email = user.Email;
            var password = user.Password;

            var users = await _Repository.UsersRepository.ReadAllAsync(u => u.Email == email);
            var user1 = users.FirstOrDefault(u => BCrypt.Net.BCrypt.Verify(password, u.Password));

            if (user1 != null)
            {
                _AppState.User = user1;

                StateHasChanged?.Invoke(this, EventArgs.Empty);

                Task.Run(InitUserCart).Wait();
                InitUserOrders();
                InitCartItems();

                if (user1.IsAdmin)
                    InitAdminState();
            }

            return user1;
        }

        public async Task<bool> LoginUserFromLocalStorage(string token)
        {
            if (string.IsNullOrEmpty(_AppState.User.Id))
            {
                var user = await _Repository.UsersRepository.ReadFirstAsync(u => u.Id == token);
                if (user != null)
                {
                    _AppState.User = user;
                    StateHasChanged?.Invoke(this, EventArgs.Empty);

                    Task.Run(InitUserCart).Wait();
                    InitUserOrders();
                    InitCartItems();

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
            _AppState.User = new User();
            _AppState.Cart = new Cart();
            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        //Alerts
        public void AddAlert(Alert alert)
        {
            _AppState.Alerts.Add(alert);
            StateHasChanged?.Invoke(this, EventArgs.Empty);
            Task.Delay(4000).ContinueWith(_ => RemoveAlert(alert));
        }

        public void RemoveAlert(Alert alert)
        {
            _AppState.Alerts.Remove(alert);
            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        //Order
        public async Task<Order?> CreateOrderFromCart()
        {
            if (string.IsNullOrEmpty(_AppState.Cart.Id) || string.IsNullOrEmpty(_AppState.User.Id))
                return await Task.FromResult(default(Order));

            try
            {
                var order = new Order
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Items = _AppState.Cart.Items,
                    UserId = _AppState.User.Id,
                    CreatedAt = DateTime.Now,
                    Comment = _AppState.Cart.Comment
                };
                var invoice = _InvoiceService.GetInvoiceHtml(order, _AppState.User);


                await _Repository.OrdersRepository.WriteAsync(order);
                await _EmailService.SendAsync("Order Invoice", invoice, _AppState.User.Email, "chabad18@hotmail.com");
                await _Repository.CartsRepository.DeleteFirstAsync(c => c.UserId == _AppState.User.Id && c.Id == _AppState.Cart.Id);

                _AppState.Cart = new Cart();
                _AppState.UserOrders.Add(order);

                StateHasChanged?.Invoke(this, EventArgs.Empty);
                return order;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> RemoveOrderAsync(Order? order)
        {
            if (order is not null && !order.IsShipped)
            {
                var res = await _Repository.OrdersRepository.DeleteFirstAsync(o => o.Id == order.Id);
                if (res)
                {
                    _AppState.UserOrders.Remove(order);
                    StateHasChanged?.Invoke(this, EventArgs.Empty);
                }

                return res;
            }

            return false;
        }

        public async Task<bool> ToggleOrderStatus(Order order, string prop)
        {
            if (string.IsNullOrEmpty(order.Id))
                return false;

            switch (prop)
            {
                case nameof(Order.IsPacked):
                    order.IsPacked = !order.IsPacked;
                    break;
                case nameof(Order.IsShipped):
                    order.IsShipped = !order.IsShipped;
                    break;
                case nameof(Order.IsPaid):
                    order.IsPaid = !order.IsPaid;
                    break;
                default:
                    return false;
            }

            var res = await _Repository.OrdersRepository.UpdateFirstAsync(o => o.Id == order.Id, order);
            if(res is not null)
                StateHasChanged?.Invoke(this, EventArgs.Empty);
            return res is not null;
        }

        public async Task<bool> AddItemToOrder(OrderItem item, string orderId)
        {
            try
            {
                var order = _AppState.UserOrders.FirstOrDefault(o => o.Id == orderId);

                if (order is null || (!_AppState.User.IsAdmin && _AppState.User.Id != order.UserId))
                    return false;
                
                order.Items.AddOrMerge(item);

                var res = await _Repository.OrdersRepository.UpdateFirstAsync(o => o.Id == order.Id, order);

                if (_AppState.User.IsAdmin)
                {
                    var aOrder = _AppState.AdminState.AllOrders.FirstOrDefault(o => o.Id == orderId);
                    aOrder = order;
                }

                StateHasChanged?.Invoke(this, EventArgs.Empty);

                return res is not null;
            }
            catch
            {
                return false;
            }
        }

        //Admin
        public async Task<bool> SendInvoiceFromAdminAsync(Order order, User user)
        {
            try
            {
                var html = _InvoiceService.GetInvoiceHtml(order, user);
                await _EmailService.SendAsync("Order Invoice", html, user.Email);

                return true;
            }
            catch
            {
                return false;
            }
        }


        async Task<bool> InitUserCart()
        {
            if (string.IsNullOrEmpty(_AppState.User.Id))
                return false;

            try
            {
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

                StateHasChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch
            {
                return false;
            }
        }

        async void InitUserOrders()
        {
            if (string.IsNullOrEmpty(_AppState.User.Id))
                return;

            var userId = _AppState.User.Id;
            var orders = await _Repository.OrdersRepository.ReadAllAsync(o => o.UserId == userId);
            _AppState.UserOrders = orders.ToList();

            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        async void InitAdminState()
        {
            var orders = await _Repository.OrdersRepository.ReadAllAsync();
            _AppState.AdminState.AllOrders = orders.ToList();

            var users = await _Repository.UsersRepository.ReadAllAsync();
            _AppState.AdminState.AllUsers = users.ToList();

            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        async void InitCartItems()
        {
            _AppState.ShopItems = await _Repository.ItemsRepository.ReadAllAsync();
        }
    }
}
