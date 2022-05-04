using SukkotMeNET.Models;

namespace SukkotMeNET.Interfaces
{
    public interface IRepositoryService
    {
        Repository<User> UsersRepository { get; }
        Repository<Item> ItemsRepository { get; }
        Repository<Cart> CartsRepository { get; }
        Repository<Order> OrdersRepository { get; }
    }
}
