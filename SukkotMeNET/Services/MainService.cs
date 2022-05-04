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

        public void AddItemToCart(OrderItem item) 
        { 
            
        }

        public async Task<bool> LoginAsync(User user)
        {
            string email = user.Email;
            string password = user.Password;
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, 10);

            var res = await _Repository.UsersRepository.ReadFirstAsync(u => u.Email == user.Email && u.Password == hashedPassword);

            if(res == null)
                return false;

            _AppState.User = res;

            return true;
        }

        public void Logout()
        {
            _AppState.User = null;
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
