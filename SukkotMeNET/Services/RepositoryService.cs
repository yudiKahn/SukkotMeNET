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

        public RepositoryService(ApplicationConfiguration appConfig)
        {
            var client = new MongoClient(appConfig.ConnectionString);
            var db = client.GetDatabase(appConfig.DatabaseName);

            UsersRepository = new UsersRepository(db.GetCollection<User>(appConfig.DBUsersCollectionName)); 
            
            ItemsRepository = new ItemsRepository(db.GetCollection<Item>(appConfig.DBItemsCollectionName));

            CartsRepository = new CartsRepository(db.GetCollection<Cart>(appConfig.DBCartsCollectionName)); 

            OrdersRepository = new OrdersRepository(db.GetCollection<Order>(appConfig.DBOrdersCollectionName));
        }
    }
}
