using MongoDB.Driver;
using System.Linq.Expressions;

namespace SukkotMeNET.Interfaces
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T?>> ReadAllAsync(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T?>> ReadAllAsync();
        Task<T?> ReadFirstAsync(Expression<Func<T, bool>> filter);

        Task<bool> DeleteFirstAsync(Expression<Func<T, bool>> filter);

        Task<T?> WriteAsync(T value);

        Task<T> UpdateFirstAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> newValue);
    }
}
