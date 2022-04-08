namespace SukkotMeNET.Interfaces
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T?>> ReadAllAsync(Func<T, bool> predicate);
        Task<IEnumerable<T?>> ReadAllAsync();
        Task<T?> ReadFirstAsync(Func<T, bool> predicate);

        Task<bool> DeleteAsync(string id);

        Task<T> WriteAsync(T value);

        Task<T> UpdateAsync(string id, T newValue);
    }
}
