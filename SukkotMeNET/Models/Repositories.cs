using MongoDB.Driver;

namespace SukkotMeNET.Models
{
    public class UsersRepository : Repository<User>
    {
        public UsersRepository(IMongoCollection<User> collection) : base(collection) { }
    }

    public class ItemsRepository : Repository<Item>
    {
        public ItemsRepository(IMongoCollection<Item> collection) : base(collection) { }
    }

    public class CartsRepository : Repository<Cart>
    {
        public CartsRepository(IMongoCollection<Cart> collection) : base(collection) { }
    }

    public class OrdersRepository : Repository<Order>
    {
        public OrdersRepository(IMongoCollection<Order> collection) : base(collection) { }
    }
}
