using Microsoft.Extensions.Options;
using SukkotMeNET.Configuration;
using MongoDB.Driver;
using SukkotMeNET.Interfaces;
using SukkotMeNET.Models;

namespace SukkotMeNET.Services
{
    public class RepositoryService : IRepositoryService
    {
        public IRepository<User> UsersRepository { get; private set; }
        public IRepository<Item> ItemsRepository { get; private set; }
        public IRepository<Cart> CartsRepository { get; private set; }
        public IRepository<Order> OrdersRepository { get; private set; }

        public RepositoryService(
            IOptions<MongodbConfig> dbConfig)
        {
            var client = new MongoClient(dbConfig.Value.ConnectionString);
            var db = client.GetDatabase(dbConfig.Value.DatabaseName);

            UsersRepository = new UsersRepository(db.GetCollection<User>(dbConfig.Value.Collections.Users)); 
            
            ItemsRepository = new ItemsRepository(db.GetCollection<Item>(dbConfig.Value.Collections.Items));

            CartsRepository = new CartsRepository(db.GetCollection<Cart>(dbConfig.Value.Collections.Carts)); 

            OrdersRepository = new OrdersRepository(db.GetCollection<Order>(dbConfig.Value.Collections.Orders));
        }
    }
}
