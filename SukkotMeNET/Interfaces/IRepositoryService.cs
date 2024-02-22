using SukkotMeNET.Models;

namespace SukkotMeNET.Interfaces
{
    public interface IRepositoryService
    {
        MongoRepository<User> UsersRepository { get; }
        MongoRepository<Item> ItemsRepository { get; }
        MongoRepository<Cart> CartsRepository { get; }
        MongoRepository<Order> OrdersRepository { get; }
    }
}
