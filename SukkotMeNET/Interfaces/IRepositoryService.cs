using SukkotMeNET.Models;

namespace SukkotMeNET.Interfaces
{
    public interface IRepositoryService
    {
        IRepository<User> UsersRepository { get; }
        IRepository<Item> ItemsRepository { get; }
        IRepository<Cart> CartsRepository { get; }
        IRepository<Order> OrdersRepository { get; }
    }
}
