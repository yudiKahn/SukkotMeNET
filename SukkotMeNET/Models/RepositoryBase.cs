using MongoDB.Driver;
using System.Linq.Expressions;

namespace SukkotMeNET.Models
{
    public class RepositoryBase<T>
    {
        readonly IMongoCollection<T> _Collection;

        public RepositoryBase(IMongoCollection<T> collection)
        {
            _Collection = collection;
        }

        public async Task<bool> DeleteFirstAsync(Expression<Func<T, bool>> predicate)
        {
            var res = await _Collection.FindOneAndDeleteAsync(predicate);
            return res != null;
        }

        public async Task<IEnumerable<T>> ReadAllAsync(Expression<Func<T, bool>> predicate)
        {
            var res = await _Collection.FindAsync(predicate);
            var items = await res.ToListAsync();
            return items ?? new List<T>();
        }

        public async Task<IEnumerable<T>> ReadAllAsync()
        {
            var res = _Collection.Find((item) => true);
            var items = await res.ToListAsync();
            return items ?? new List<T>();
        }

        public async Task<T?> ReadFirstAsync(Expression<Func<T, bool>> predicate)
        {
            var res = await _Collection.FindAsync(predicate);
            var documents = await res.FirstOrDefaultAsync();
            return documents;
        }

        public async Task<T?> UpdateFirstAsync(Expression<Func<T, bool>> filter, T newValue, bool writeIfNotFound = true)
        {
            try
            {
                await _Collection.ReplaceOneAsync<T>(filter, newValue, new ReplaceOptions()
                {
                    IsUpsert = writeIfNotFound,
                    
                });
                return newValue;
            }
            catch
            {
                return default;
            }
        }

        public async Task<T?> WriteAsync(T value)
        {
            try
            {
                await _Collection.InsertOneAsync(value);
                return value;
            }
            catch
            {
                return default;
            }
        }
    }
}
