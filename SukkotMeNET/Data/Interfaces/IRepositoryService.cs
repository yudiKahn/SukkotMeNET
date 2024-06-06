using SukkotMeNET.Data.Entities;

namespace SukkotMeNET.Data.Interfaces
{
    public interface IRepositoryService
    {
        MongoRepository<UserEntity> UsersRepository { get; }
        MongoRepository<ProductEntity> ItemsRepository { get; }
        MongoRepository<CartEntity> CartsRepository { get; }
        MongoRepository<OrderEntity> OrdersRepository { get; }
    }
}
