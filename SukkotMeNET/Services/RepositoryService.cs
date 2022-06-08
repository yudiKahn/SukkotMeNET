using Microsoft.Extensions.Options;
using SukkotMeNET.Configuration;
using MongoDB.Driver;
using SukkotMeNET.Interfaces;
using SukkotMeNET.Models;

namespace SukkotMeNET.Services
{
    public class RepositoryService : IRepositoryService
    {
        public RepositoryBase<User> UsersRepository { get; private set; }
        public RepositoryBase<Item> ItemsRepository { get; private set; }
        public RepositoryBase<Cart> CartsRepository { get; private set; }
        public RepositoryBase<Order> OrdersRepository { get; private set; }

        public RepositoryService(MongodbConfig dbConfig)
        {
            Console.WriteLine($"connectionStr: {dbConfig.ConnectionString}");

            var client = new MongoClient(dbConfig.ConnectionString);
            var db = client.GetDatabase(dbConfig.DatabaseName);

            UsersRepository = new UsersRepository(db.GetCollection<User>(dbConfig.Collections.Users)); 
            
            ItemsRepository = new ItemsRepository(db.GetCollection<Item>(dbConfig.Collections.Items));

            CartsRepository = new CartsRepository(db.GetCollection<Cart>(dbConfig.Collections.Carts)); 

            OrdersRepository = new OrdersRepository(db.GetCollection<Order>(dbConfig.Collections.Orders));
        }
    }
}
