using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Text.Json;

namespace SukkotMeNET.Interfaces;

public interface IRepository<T>
{
    Task<T> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    
    Task CreateAsync(T entity);

    Task<bool> UpdateFirstAsync(Func<T, bool> predicate, T entity);
    Task<bool> DeleteAsync(string id);

}

public class MongoRepository<T> : IRepository<T>
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<T>(collectionName);
    }

    public async Task<T> GetByIdAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("_id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task CreateAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task<bool> UpdateFirstAsync(Func<T, bool> predicate, T entity)
    {
        var f = new ExpressionFilterDefinition<T>(t => predicate(t));
        var u = new ObjectUpdateDefinition<T>(entity);
        var res = await _collection.UpdateOneAsync(f, u);
        return res.IsAcknowledged && res.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("_id", id);
        var result = await _collection.DeleteOneAsync(filter);

        return result.IsAcknowledged && result.DeletedCount > 0;
    }
}

public class FileSystemRepository<T> : IRepository<T>
{
    private readonly string _directoryPath;

    public FileSystemRepository(string directoryPath)
    {
        _directoryPath = directoryPath;
    }

    private string GetFilePath(string id) => Path.Combine(_directoryPath, id + ".json");

    public async Task<T> GetByIdAsync(string id)
    {
        var filePath = GetFilePath(id);
        if (!File.Exists(filePath))
            return default;

        var json = await File.ReadAllTextAsync(filePath);

        return JsonSerializer.Deserialize<T>(json)!;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var files = Directory.GetFiles(_directoryPath, "*.json");
        var entities = new List<T>();

        foreach (var file in files)
        {
            var json = await File.ReadAllTextAsync(file);
            var entity = JsonSerializer.Deserialize<T>(json)!;
            entities.Add(entity);
        }

        return entities;
    }

    public async Task CreateAsync(T entity)
    {
        var id = Guid.NewGuid().ToString();
        var filePath = GetFilePath(id);
        var json = JsonSerializer.Serialize(entity);
        await File.WriteAllTextAsync(filePath, json);
    }

    public Task<bool> UpdateFirstAsync(Func<T, bool> predicate, T entity)
    {
        throw new NotImplementedException();
    }


    public async Task<bool> DeleteAsync(string id)
    {
        var filePath = GetFilePath(id);
        if (!File.Exists(filePath))
            return false;

        File.Delete(filePath);
        return true;
    }
}