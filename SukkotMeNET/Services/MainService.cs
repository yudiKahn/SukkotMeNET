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

        public void AddItemToCart(OrderItem item) 
        { 
            
        }

        public async Task<bool> LoginAsync(User user)
        {
            string email = user.Email;
            string password = user.Password;

            var users = await _Repository.UsersRepository.ReadAllAsync(u => u.Email == email);
            var res = users.FirstOrDefault(u => BCrypt.Net.BCrypt.Verify(password, u?.Password));

            if(res == null)
                return false;

            _AppState.User = res;
            StateHasChanged?.Invoke(this, EventArgs.Empty);

            return true;
        }

        public void Logout()
        {
            _AppState.User = null;
            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AddAlert(Alert alert)
        {
            _AppState.Alerts.Add(alert);
            StateHasChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveAlert(Alert alert) => _AppState.Alerts.Remove(alert);

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //TODO: init cart, orders, cached user,
            return Task.CompletedTask;
        }
    }
}
