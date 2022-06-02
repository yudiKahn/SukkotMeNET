using SukkotMeNET.Models;

namespace SukkotMeNET.Interfaces
{
    public interface IRepositoryService
    {
        RepositoryBase<User> UsersRepository { get; }
        RepositoryBase<Item> ItemsRepository { get; }
        RepositoryBase<Cart> CartsRepository { get; }
        RepositoryBase<Order> OrdersRepository { get; }
    }
}
