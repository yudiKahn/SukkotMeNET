using SukkotMeNET.Configuration;
using MongoDB.Driver;
using SukkotMeNET.Interfaces;
using SukkotMeNET.Models;

namespace SukkotMeNET.Services;

public class RepositoryService : IRepositoryService
{
    public IRepository<User> UsersRepository { get; }
    public IRepository<Item> ItemsRepository { get; }
    public IRepository<Cart> CartsRepository { get; }
    public IRepository<Order> OrdersRepository { get; }

    public RepositoryService(ApplicationConfiguration appConfig)
    {
        var client = new MongoClient(appConfig.ConnectionString);
        IMongoDatabase? db = client.GetDatabase(appConfig.DatabaseName);

        UsersRepository =  CreateMongoRepo<User>(db, appConfig.DBUsersCollectionName); 
        ItemsRepository =  CreateMongoRepo<Item>(db, appConfig.DBItemsCollectionName);
        CartsRepository =  CreateMongoRepo<Cart>(db, appConfig.DBCartsCollectionName); 
        OrdersRepository = CreateMongoRepo<Order>(db, appConfig.DBOrdersCollectionName);
    }

    IRepository<T> CreateMongoRepo<T>(IMongoDatabase db, string table)
    {
        return new Interfaces.MongoRepository<T>(db, table);
    }
}

