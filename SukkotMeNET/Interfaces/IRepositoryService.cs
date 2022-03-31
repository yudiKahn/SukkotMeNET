using SukkotMeNET.Models;

namespace SukkotMeNET.Interfaces
{
    public interface IRepositoryService
    {
        IRepository<User> UserRepository { get; }
    }
}
