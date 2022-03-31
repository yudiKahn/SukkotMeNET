using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SukkotMeNET.Configuration;
using SukkotMeNET.Interfaces;
using SukkotMeNET.Models;

namespace SukkotMeNET.Services
{
    public class UserRepository : IRepository<User>
    {
        readonly IMongoCollection<User> _UserRepo;
        public UserRepository(IMongoCollection<User> repo)
        {
            _UserRepo = repo;
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<User?> ReadAsync(string id)
        {
            var res = await _UserRepo.FindAsync(u => u.Id == id);
            var user = res.FirstOrDefault();
            return user;
        }

        public async Task<IEnumerable<User?>> ReadAsync()
        {
            var res = _UserRepo.Find(x => true);
            var users = await res.ToListAsync();
            return users;
        }

        public Task<User> UpdateAsync(string id, User newValue)
        {
            throw new NotImplementedException();
        }

        public Task<User> WriteAsync(User value)
        {
            throw new NotImplementedException();
        }
    }
}
