using SukkotMeNET.Configuration;
using MongoDB.Driver;
using SukkotMeNET.Interfaces;
using SukkotMeNET.Models;

namespace SukkotMeNET.Services
{
    public class RepositoryService : IRepositoryService
    {
        public MongoRepository<User> UsersRepository { get; }
        public MongoRepository<Item> ItemsRepository { get; }
        public MongoRepository<Cart> CartsRepository { get; }
        public MongoRepository<Order> OrdersRepository { get; }

        public RepositoryService(ApplicationConfiguration appConfig)
        {
            var client = new MongoClient(appConfig.ConnectionString);
            var db = client.GetDatabase(appConfig.DatabaseName);

            UsersRepository = new MongoRepository<User>(db.GetCollection<User>(appConfig.DBUsersCollectionName)); 
            
            ItemsRepository = new MongoRepository<Item>(db.GetCollection<Item>(appConfig.DBItemsCollectionName));

            CartsRepository = new MongoRepository<Cart>(db.GetCollection<Cart>(appConfig.DBCartsCollectionName)); 

            OrdersRepository = new MongoRepository<Order>(db.GetCollection<Order>(appConfig.DBOrdersCollectionName));
        }
    }
}
