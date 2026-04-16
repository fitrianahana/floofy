namespace floofy.Data;

using floofy.Models;
public interface IRepository<T> where T : Entity, new()
{
  // CREATE OPERATIONS
  Task InsertAsync(T entity);
  Task InsertAllAsync(IEnumerable<T> entities);

  // READ OPERATIONS
  Task<T?> GetByIdAsync(Guid id);
  Task<List<T>> GetAllAsync();
  Task<List<T>> QueryAsync(Func<T, bool> predicate);
  Task<int> CountAsync();
  Task<bool> ExistsAsync(Guid id);

  // UPDATE OPERATIONS
  Task UpdateAsync(T entity);
  Task UpdateAllAsync(IEnumerable<T> entities);

  // DELETE OPERATIONS
  Task DeleteAsync(Guid id);
  Task DeleteAsync(T entity);
  Task SoftDeleteAsync(Guid id);
}