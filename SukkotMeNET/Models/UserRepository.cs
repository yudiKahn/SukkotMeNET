using MongoDB.Driver;
using SukkotMeNET.Interfaces;
using System.Linq.Expressions;

namespace SukkotMeNET.Models
{
    public class UserRepository : IRepository<User>
    {
        readonly IMongoCollection<User> _UserCollection;
        public UserRepository(IMongoCollection<User> collection)
        {
            _UserCollection = collection;
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User?>> ReadAllAsync(Func<User, bool> predicate)
        {
            var res = _UserCollection.Find(u => predicate(u));
            var users = await res.ToListAsync();
            return users;
        }

        public async Task<IEnumerable<User?>> ReadAllAsync()
        {
            var res = _UserCollection.Find(u => true);
            var users = await res.ToListAsync();
            return users;
        }

        public async Task<User?> ReadFirstAsync(Expression<Func<User, bool>> predicate)
        {
            try
            {
                var res = await _UserCollection.FindAsync(predicate);
                var user = res.FirstOrDefault();
                return user;
            }
            catch (Exception ex)
            {
                throw;
            }
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
