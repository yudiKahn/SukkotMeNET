using System.Text;
using Microsoft.Extensions.Options;
using SukkotMeNET.Configuration;
using MongoDB.Driver;
using SukkotMeNET.Interfaces;
using SukkotMeNET.Models;

namespace SukkotMeNET.Services
{
    public class RepositoryService : IRepositoryService
    {
        public RepositoryBase<User> UsersRepository { get; }
        public RepositoryBase<Item> ItemsRepository { get; }
        public RepositoryBase<Cart> CartsRepository { get; }
        public RepositoryBase<Order> OrdersRepository { get; }

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
