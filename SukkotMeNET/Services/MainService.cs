using SukkotMeNET.Interfaces;
using SukkotMeNET.Models;
using System.IdentityModel.Tokens.Jwt;
using static BCrypt.Net.BCrypt;

namespace SukkotMeNET.Services
{
    public class MainService : BackgroundService
    {
        readonly AppStateService _AppState;
        readonly IRepositoryService _Repository;

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
            var hashedPassword = HashPassword(user.Password);

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

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //TODO: init cart, orders, cached user,
            return Task.CompletedTask;
        }
    }
}
