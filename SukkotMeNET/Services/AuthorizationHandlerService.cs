using Microsoft.AspNetCore.Authorization;
using SukkotMeNET.Models;

namespace SukkotMeNET.Services
{
    public class AuthorizationHandlerService : IAuthorizationHandler
    {
        private readonly AppStateService _AppStateService;
        public AuthorizationHandlerService(AppStateService appState)
        {
            _AppStateService = appState;
        }
        
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (context.User != null && _AppStateService.User != null)
            {
                var pendingRequirements = context.PendingRequirements.ToList();

                foreach (var requirement in pendingRequirements)
                {
                    var userId = context.Resource?.ToString();

                    var isAuth = requirement switch
                    {
                        AdminRequirement => _AppStateService.User.IsAdmin,
                        UserRequirement => _AppStateService.User.Id == userId, //TODO userId == mongoObjectId
                        _ => false
                    };

                    if (isAuth)
                    {
                        context.Succeed(requirement);
                    }
                }

            }

            return Task.CompletedTask;
        }
    }
}
