using Microsoft.AspNetCore.Authorization;

namespace IEsrog.Models
{
    public record AdminRequirement : IAuthorizationRequirement;
    public record UserRequirement : IAuthorizationRequirement;
    public record GuestRequirement : IAuthorizationRequirement;

}
