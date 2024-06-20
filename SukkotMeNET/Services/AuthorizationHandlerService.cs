using IEsrog.Models;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;

namespace IEsrog.Services
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
                        AdminRequirement => _AppStateService.User.IsAdmin || _AppStateService.AdminState.CurrentAdminUser?.IsAdmin == true,
                        UserRequirement => ObjectId.TryParse(_AppStateService.User.Id, out var id),
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
