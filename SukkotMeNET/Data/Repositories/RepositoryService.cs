﻿using MongoDB.Driver;
using SukkotMeNET.Configuration;
using SukkotMeNET.Data.Entities;
using SukkotMeNET.Data.Interfaces;

namespace SukkotMeNET.Data.Repositories;

public class RepositoryService : IRepositoryService
{
    public MongoRepository<UserEntity> UsersRepository { get; }
    public MongoRepository<ProductEntity> ItemsRepository { get; }
    public MongoRepository<CartEntity> CartsRepository { get; }
    public MongoRepository<OrderEntity> OrdersRepository { get; }

    public RepositoryService(ApplicationConfiguration appConfig)
    {
        var client = new MongoClient(appConfig.ConnectionString);
        var db = client.GetDatabase(appConfig.DatabaseName);

        UsersRepository = new MongoRepository<UserEntity>(db.GetCollection<UserEntity>(appConfig.DBUsersCollectionName));

        ItemsRepository = new MongoRepository<ProductEntity>(db.GetCollection<ProductEntity>(appConfig.DBItemsCollectionName));

        CartsRepository = new MongoRepository<CartEntity>(db.GetCollection<CartEntity>(appConfig.DBCartsCollectionName));

        OrdersRepository = new MongoRepository<OrderEntity>(db.GetCollection<OrderEntity>(appConfig.DBOrdersCollectionName));
    }
}