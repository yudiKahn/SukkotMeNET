using Microsoft.Extensions.Options;
using SukkotMeNET.Configuration;
using MongoDB.Driver;
using SukkotMeNET.Interfaces;
using SukkotMeNET.Models;

namespace SukkotMeNET.Services
{
    public class RepositoryService : IRepositoryService
    {
        public IRepository<User> UserRepository { get; private set; }

        public RepositoryService(
            IOptions<MongodbConfig> dbConfig)
        {
            var client = new MongoClient(dbConfig.Value.ConnectionString);
            var db = client.GetDatabase(dbConfig.Value.DatabaseName);

            UserRepository = new UserRepository(db.GetCollection<User>(dbConfig.Value.Collections.Users));
        }
    }
}
