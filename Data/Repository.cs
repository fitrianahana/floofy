namespace floofy.Data;

using SQLite;
using floofy.Models;

public class Repository<T> : IRepository<T> where T : Entity, new()
{
  private readonly SQLiteAsyncConnection _db;

  public Repository(AppDatabase appDatabase)
  {
    _db = appDatabase.Database;
  }

  // READ OPERATIONS
  public async Task<T?> GetByIdAsync(Guid id)
  {
    return await _db.FindAsync<T>(id);
  }

  public async Task<List<T>> GetAllAsync()
  {
    return await _db.Table<T>().ToListAsync();
  }

  public async Task<List<T>> QueryAsync(Func<T, bool> predicate)
  {
    var allItems = await _db.Table<T>().ToListAsync();
    return allItems.Where(predicate).ToList();
  }

  public async Task<int> CountAsync()
  {
    return await _db.Table<T>().CountAsync();
  }

  public async Task<bool> ExistsAsync(Guid id)
  {
    var item = await GetByIdAsync(id);
    return item != null;
  }

  // CREATE OPERATIONS
  public async Task InsertAsync(T entity)
  {
    entity.CreatedAt = DateTime.UtcNow;
    entity.UpdatedAt = DateTime.UtcNow;
    await _db.InsertAsync(entity);
  }

  public async Task InsertAllAsync(IEnumerable<T> entities)
  {
    var now = DateTime.UtcNow;
    foreach (var entity in entities)
    {
      entity.CreatedAt = now;
      entity.UpdatedAt = now;
    }
    await _db.InsertAllAsync(entities);
  }

  // UPDATE OPERATIONS
  public async Task UpdateAsync(T entity)
  {
    entity.MarkAsUpdated();
    await _db.UpdateAsync(entity);
  }

  public async Task UpdateAllAsync(IEnumerable<T> entities)
  {
    foreach (var entity in entities)
    {
      entity.MarkAsUpdated();
    }
    await _db.UpdateAllAsync(entities);
  }

  // DELETE OPERATIONS
  public async Task DeleteAsync(Guid id)
  {
    var item = await GetByIdAsync(id);
    if (item != null)
      await _db.DeleteAsync(item);
  }

  public async Task DeleteAsync(T entity)
  {
    await _db.DeleteAsync(entity);
  }

  public async Task SoftDeleteAsync(Guid id)
  {
    var item = await GetByIdAsync(id);
    if (item != null)
    {
      item.MarkAsDeleted();
      await _db.UpdateAsync(item);
    }
  }
}