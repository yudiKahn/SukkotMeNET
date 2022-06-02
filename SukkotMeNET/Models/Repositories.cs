using MongoDB.Driver;

namespace SukkotMeNET.Models
{
    public class UsersRepository : RepositoryBase<User>
    {
        public UsersRepository(IMongoCollection<User> collection) : base(collection) { }
    }

    public class ItemsRepository : RepositoryBase<Item>
    {
        public ItemsRepository(IMongoCollection<Item> collection) : base(collection) { }
    }

    public class CartsRepository : RepositoryBase<Cart>
    {
        public CartsRepository(IMongoCollection<Cart> collection) : base(collection) { }
    }

    public class OrdersRepository : RepositoryBase<Order>
    {
        public OrdersRepository(IMongoCollection<Order> collection) : base(collection) { }
    }
}
