using IEsrog.Data.Entities;
using IEsrog.Data.Interfaces;
using IEsrog.Models;
using MongoDB.Bson;
using IEsrog.Extensions;

namespace IEsrog.Services
{
    public class MainStateService
    {
        readonly AppStateService _AppState;
        readonly IRepositoryService _Repository;
        readonly EmailService _EmailService;
        readonly InvoiceService _InvoiceService;
        readonly FireAndForgetService _FireAndForgetService;

        public event EventHandler? StateHasChanged;

        public MainStateService(
            AppStateService appState,
            IRepositoryService repositoryService,
            EmailService emailService,
            InvoiceService invoiceService, 
            FireAndForgetService fireAndForgetService)
        {
            _AppState = appState;
            _Repository = repositoryService;
            _EmailService = emailService;
            _InvoiceService = invoiceService;
            _FireAndForgetService = fireAndForgetService;

            Task.Run(UpdProd);
        }

        static bool once = true;
        async Task UpdProd()
        {
            try
            {
                await Task.Delay(1000);
                if (!once) return;

                once = false;


                var items = await _Repository.ProductsRepository.ReadAllAsync();
                var grp = (byte)0;

                var add = new List<ProductEntity>();
                foreach (var i in items)
                {
                    
                }

                foreach (var a in add)
                {
                    await _Repository.ProductsRepository.UpdateFirstAsync(p => p.Id == a.Id, a, false);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Cart
        public async Task<bool> AddItemToCart(OrderItem item, string? userId = null, bool toOverride = false)
        {
            try
            {
                var itemClone = item.Clone();

                if (itemClone == null)
                    throw new Exception("Unknown error (0x1), Please contact developer");
                //if (itemClone.Qty < 1)
                //    throw new Exception("Quantity cannot be less then one");


                _AppState.Cart.Items.AddOrMerge(itemClone, toOverride);

                await _Repository.CartsRepository.UpdateFirstAsync(
                     c => c.UserId == (userId ?? _AppState.User.Id),
                     _AppState.Cart.ToEntity());

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
            await _Repository.CartsRepository.UpdateFirstAsync(c => c.Id == _AppState.Cart.Id, _AppState.Cart.ToEntity());
            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        public async void ClearCart()
        {
            _AppState.Cart.Items.Clear();
            await _Repository.CartsRepository.DeleteFirstAsync(c => c.Id == _AppState.Cart.Id);
            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        public async void SaveCurrentCart()
        {
            await _Repository.CartsRepository.UpdateFirstAsync(c => c.UserId == _AppState.User.Id, _AppState.Cart.ToEntity());
        }

        public async Task CreateDuplicateOrder()
        {
            var items = GetLastYearOrder();
            if (items.Count == 0) return;

            _AppState.Cart.Items.Clear();
            _AppState.Cart.Items.AddOrMergeRange(items.ToArray());
            await _Repository.CartsRepository.UpdateFirstAsync(c => c.UserId == _AppState.User.Id, _AppState.Cart.ToEntity());
        }

        //Login
        public async Task<User?> LoginAsync(User user)
        {
            var email = user.Email;
            var password = user.Password;

            var users = await _Repository.UsersRepository.ReadAllAsync(u => u.Email.ToLower() == email.ToLower());
            var user1 = users.FirstOrDefault(u => BCrypt.Net.BCrypt.Verify(password, u.Password));

            if (user1 != null)
            {
                _AppState.User = user1.ToModel();

                StateHasChanged?.Invoke(this, EventArgs.Empty);

                Task.Run(InitUserCart).Wait();
                InitUserOrders();
                InitCartItems();

                if (user1.IsAdmin)
                    InitAdminState();
            }

            return user1?.ToModel();
        }

        public async Task<bool> LoginUserFromLocalStorage(string token)
        {
            if (string.IsNullOrEmpty(_AppState.User.Id))
            {
                var user = await _Repository.UsersRepository.ReadFirstAsync(u => u.Id == token);
                if (user != null)
                {
                    _AppState.User = user.ToModel();
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
            _AppState.Cart.Id = ObjectId.GenerateNewId().ToString();
            _AppState.Cart.UserId = _AppState.User.Id;
            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        //Register
        public async Task RegisterAsync(User user)
        {
            var userExist = await _Repository.UsersRepository.ReadFirstAsync(u => u.Email == user.Email);
            if (userExist is not null)
                throw new Exception("User with this email address already exist");

            var hashPass = BCrypt.Net.BCrypt.HashPassword(user.Password);
            if (hashPass is null)
                throw new Exception("An error accord while saving user");

            user.Password = hashPass;
            var user1 = await _Repository.UsersRepository.WriteAsync(user.ToEntity());

            if (user1 is null)
                throw new Exception("An error accord while saving user");

            if (_AppState.AdminState.AllUsers.Any())
                _AppState.AdminState.AllUsers.Add(user1.ToModel());
        }

        //Alerts
        public void AddAlert(Alert alert)
        {
            new Thread(() =>
            {
                _AppState.Alerts.Add(alert);
                StateHasChanged?.Invoke(this, EventArgs.Empty);
                Task.Delay(4000).ContinueWith(_ => RemoveAlert(alert));
            }).Start();
        }

        public void RemoveAlert(Alert alert)
        {
            _AppState.Alerts.Remove(alert);
            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        //Order
        public async Task<bool> RecreateOrderFromOldOrder(Order oldOrder)
        {
            try
            {
                if (string.IsNullOrEmpty(_AppState.User.Id))
                    throw new Exception("Failed to create order. Please try again later");

                var order = new Order
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Items = oldOrder.Items.Select(i => i).ToList(),
                    UserId = _AppState.User.Id,
                    CreatedAt = DateTime.Now,
                };

                var o = await _Repository.OrdersRepository.WriteAsync(order.ToEntity());
                if (o is not null)
                {
                    _AppState.UserOrders.Add(o.ToModel());
                    return true;
                }

                throw new Exception("Failed to create order. Please try again later");
            }
            catch (Exception e)
            {
                AddAlert(new Alert("Error", e.Message, AlertType.Error));
                return false;
            }
        }

        public async Task<Order?> CreateOrderFromCart()
        {
            if (string.IsNullOrEmpty(_AppState.Cart.Id) || string.IsNullOrEmpty(_AppState.User.Id))
                return null;

            try
            {
                var user = _AppState.ForUser ?? _AppState.User;

                var order = new Order
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Items = _AppState.Cart.Items,
                    UserId = user.Id,
                    CreatedAt = DateTime.Now,
                    Comment = _AppState.Cart.Comment
                };

                var invoice = _InvoiceService.GetInvoiceHtml(order, user);

                var o = await _Repository.OrdersRepository.WriteAsync(order.ToEntity());
                
                _FireAndForgetService.Fire(new FireAndForgetSendEmailData(
                    "Order Invoice", invoice, [user.Email], "chabad18@hotmail.com"));
                //_ = await _EmailService.SendAsync(
                //    "Order Invoice", invoice, bcc: "chabad18@hotmail.com", [user.Email]);
               
                await _Repository.CartsRepository.DeleteFirstAsync(c => c.Id == _AppState.Cart.Id);

                _AppState.ForUser = null;
                _AppState.Cart = new Cart();

                _AppState.Cart.Id = ObjectId.GenerateNewId().ToString();
                _AppState.Cart.UserId = _AppState.User.Id;
                _AppState.UserOrders.Add(order);

                StateHasChanged?.Invoke(this, EventArgs.Empty);
                return o?.ToModel();
            }
            catch
            {
                return null;
            }
        }

        public async Task<Order?> CreateOrder(Order order, User user)
        {
            try
            {
                if (!ObjectId.TryParse(user.Id, out _))
                {
                    if (!_AppState.User.IsAdmin) return null;

                    user = _AppState.User;
                    order.UserId = user.Id;
                }
                var invoice = _InvoiceService.GetInvoiceHtml(order, user);

                var o = await _Repository.OrdersRepository.WriteAsync(order.ToEntity());
                if (o is null) throw new Exception("Failed to write order");
                _ = await _EmailService.SendAsync(
                    "Order Invoice", invoice, "chabad18@hotmail.com", [user.Email]);

                var model = o?.ToModel();
                _AppState.AdminState?.AllOrders.Add(model);
                StateHasChanged?.Invoke(this, EventArgs.Empty);
                
                return model;
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
                    _AppState.AdminState?.AllOrders?.Remove(order);
                    if (order.UserId == _AppState.User.Id)
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

            var res = await _Repository.OrdersRepository.UpdateFirstAsync(o => o.Id == order.Id, order.ToEntity());
            if (res is not null)
                StateHasChanged?.Invoke(this, EventArgs.Empty);
            return res is not null;
        }

        public async Task<bool> AddItemToOrder(OrderItem item, string orderId)
        {
            try
            {
                var order = _AppState.UserOrders.FirstOrDefault(o => o.Id == orderId);
                if (_AppState.User.Id == order?.UserId)
                {
                    order?.Items.AddOrMerge(item);
                }
                else
                {
                    if (!_AppState.User.IsAdmin)
                        return false;

                    order = _AppState.AdminState.AllOrders.FirstOrDefault(o => o.Id == orderId);
                    order?.Items.AddOrMerge(item);
                }

                if (order != null)
                {
                    await _Repository.OrdersRepository.UpdateFirstAsync(o => o.Id == orderId, order.ToEntity());
                }
                
                StateHasChanged?.Invoke(this, EventArgs.Empty);

                return order is not null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SetShipmentPrice(string oId, double price)
        {
            var order = _AppState.AdminState.AllOrders.FirstOrDefault(o => o.Id == oId);
            if (order is null) return false;

            order.ShippingCost = price;
            await _Repository.OrdersRepository.UpdateFirstAsync(o => o.Id == oId, order.ToEntity(), false);
            return true;
        }

        public async Task<bool> RemoveItemFromOrder(Order? order, OrderItem item)
        {
            if (order is null) return false;

            order.Items.Remove(item);

            if (_AppState.User.IsAdmin)
                _AppState.AdminState.AllOrders.First(o => o.Id == order.Id).Items.Remove(item);
            if (_AppState.User.Id == order.UserId)
                _AppState.UserOrders.First(o => o.Id == order.Id).Items.Remove(item);

            await _Repository.OrdersRepository.UpdateFirstAsync(o => o.Id == order.Id, order.ToEntity());

            StateHasChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public List<OrderItem> GetLastYearOrder()
        {
            var res = new List<OrderItem>();

            res.AddOrMergeRange(_AppState.UserOrders
                .Where(o => o.CreatedAt.Year >= DateTime.Now.Year - 1)
                .SelectMany(o => o.Items, (_, b) => b.Clone())
                .ToArray());
            return res;
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

        public async Task<bool> RemoveUserAsync(User user)
        {
            if (!_AppState.User.IsAdmin || user.IsAdmin)
                return false;

            var res = await _Repository.UsersRepository.DeleteFirstAsync(u => u.Id == user.Id);

            if (res)
            {
                _AppState.AdminState.AllUsers.Remove(user);
                StateHasChanged?.Invoke(this, EventArgs.Empty);
            }

            return res;
        }

        public async Task<bool> ChangeUserPassword(string email, string newPass)
        {
            email = email.ToLower();

            if (!_AppState.User.IsAdmin) return false;

            var user = _AppState.AdminState.AllUsers.FirstOrDefault(u => u.Email.ToLower() == email);
            if (user == null) return false;

            var hashPass = BCrypt.Net.BCrypt.HashPassword(newPass);
            user.Password = hashPass;

            var u1 = await _Repository.UsersRepository.UpdateFirstAsync(u => u.Email.ToLower() == email, user.ToEntity());

            return u1?.Password == hashPass;
        }

        public async Task<Dictionary<string, int>> GetStockData()
        {
            var from = DateTime.Now.AddMonths(-4);
            
            var orders = await _Repository.OrdersRepository.ReadAllAsync(o => o.CreatedAt >= from);

            var prod = _AppState.Products.ToDictionary(k => k.Id);
            
            var res = new Dictionary<string, int>();

            foreach (var oi in orders.SelectMany(o => o.Items))
            {
                if (string.IsNullOrEmpty(oi.ProductId)) continue;
                
                var p = prod[oi.ProductId];
                if (p.Includes?.Any() == true)
                {
                    var opt = oi.Option;
                    var inc = p.Includes
                        .Select(i => prod[i.ProductId].ToModel(opt, oi.PriceType, i.Qty))
                        .ToArray();

                    foreach (var i in inc)
                    {
                        res.TryGetValue(i.Name, out var x);
                        res[Key(i)] = x + i.Qty;
                    }
                }
                else
                {
                    res.TryGetValue(oi.Name, out var x);
                    res[Key(oi)] = x + oi.Qty;
                }
            }

            return res;
        }

        string Key(OrderItem oi)
        {
            var opt = string.IsNullOrWhiteSpace(oi.Option) ? string.Empty : " " + oi.Option;
            var pt = string.IsNullOrWhiteSpace(oi.PriceType) ? string.Empty : " " + oi.PriceType;
            return $"{oi.Name}{opt}{pt}";
        }
        
        string Key(OrderItemEntity oi)
        {
            var opt = string.IsNullOrWhiteSpace(oi.Option) ? string.Empty : " " + oi.Option;
            var pt = string.IsNullOrWhiteSpace(oi.PriceType) ? string.Empty : " " + oi.PriceType;
            return $"{oi.Name}{opt}{pt}";
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
                    _AppState.Cart = cart.ToModel();
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
            _AppState.UserOrders = orders.Select(o => o.ToModel()).ToList();

            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        async void InitAdminState()
        {
            var orders = await _Repository.OrdersRepository.ReadAllAsync();
            _AppState.AdminState.AllOrders = orders.Select(o => o.ToModel()).ToList();

            var users = await _Repository.UsersRepository.ReadAllAsync();
            _AppState.AdminState.AllUsers = users.Select(o => o.ToModel()).ToList();

            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        async void InitCartItems()
        {
            var e = await _Repository.ProductsRepository.ReadAllAsync();
            _AppState.Products = e.Select(i => i.ToModel()).ToList();
            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task LoginAsUser(User user)
        {
            try
            {
                if (_AppState.AdminState.CurrentAdminUser is null)
                {
                    _AppState.AdminState.CurrentAdminUser = _AppState.User.Clone();
                }

                _AppState.User = user;

                StateHasChanged?.Invoke(this, EventArgs.Empty);

                await InitUserCart();
                InitUserOrders();

                StateHasChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception _)
            {
                AddAlert(new Alert("Error", "Failed to switch user.", AlertType.Error));
            }
        }

        public async Task LoginBack()
        {
            if (_AppState.AdminState.CurrentAdminUser is null)
                return;

            _AppState.User = _AppState.AdminState.CurrentAdminUser.Clone();

            StateHasChanged?.Invoke(this, EventArgs.Empty);

            await InitUserCart();
            InitUserOrders();

            StateHasChanged?.Invoke(this, EventArgs.Empty);


            _AppState.AdminState.CurrentAdminUser = null;
        }

        public async Task UpdateOrder(Order order, User user)
        {
            var newOrder = await _Repository.OrdersRepository.UpdateFirstAsync(
                o => o.Id == order.Id,
                order.ToEntity(), false);

            if (_AppState.User.IsAdmin)
            {
                var inx = _AppState.AdminState.AllOrders.FindIndex(o => o.Id == order.Id);
                _AppState.AdminState.AllOrders[inx] = order;
            }
            else
            {
                var inx = _AppState.UserOrders.FindIndex(o => o.Id == order.Id);
                _AppState.UserOrders[inx] = order;
            }
        }
        
        public async Task<BackupResult> Backup()
        {
            var users = await _Repository.UsersRepository.ReadAllAsync();
            var usersJson = System.Text.Json.JsonSerializer.Serialize(users);

            var items = await _Repository.ProductsRepository.ReadAllAsync();
            var itemsJson = System.Text.Json.JsonSerializer.Serialize(items);

            var carts = await _Repository.CartsRepository.ReadAllAsync();
            var cartsJson = System.Text.Json.JsonSerializer.Serialize(carts);

            var orders = await _Repository.OrdersRepository.ReadAllAsync();
            var ordersJson = System.Text.Json.JsonSerializer.Serialize(orders);

            return new BackupResult(ordersJson, itemsJson, cartsJson, usersJson);
        }
    }
}
