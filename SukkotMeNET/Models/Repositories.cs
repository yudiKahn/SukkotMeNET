using MongoDB.Driver;

namespace SukkotMeNET.Models
{
    public class UsersRepository : AbstractRepository<User>
    {
        public UsersRepository(IMongoCollection<User> collection) : base(collection) { }
    }

    public class ItemsRepository : AbstractRepository<Item>
    {
        public ItemsRepository(IMongoCollection<Item> collection) : base(collection) { }
    }

    public class CartsRepository : AbstractRepository<Cart>
    {
        public CartsRepository(IMongoCollection<Cart> collection) : base(collection) { }
    }

    public class OrdersRepository : AbstractRepository<Order>
    {
        public OrdersRepository(IMongoCollection<Order> collection) : base(collection) { }
    }
}
