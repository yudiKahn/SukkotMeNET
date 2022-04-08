using Microsoft.AspNetCore.Authorization;

namespace SukkotMeNET.Models
{
    public record AdminRequirement : IAuthorizationRequirement;
    public record UserRequirement : IAuthorizationRequirement;
}
