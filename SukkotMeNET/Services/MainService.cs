using SukkotMeNET.Interfaces;
using SukkotMeNET.Models;

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

        public async void Login(User user)
        {
            var res = await _Repository.UserRepository.ReadFirstAsync(u => 
                u.Email == user.Email);

            _AppState.User = res;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
