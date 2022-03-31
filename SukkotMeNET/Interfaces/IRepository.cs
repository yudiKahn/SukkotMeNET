namespace SukkotMeNET.Interfaces
{
    public interface IRepository<T>
    {
        Task<T?> ReadAsync(string id);
        Task<IEnumerable<T?>> ReadAsync();

        Task<bool> DeleteAsync(string id);

        Task<T> WriteAsync(T value);

        Task<T> UpdateAsync(string id, T newValue);
    }
}
