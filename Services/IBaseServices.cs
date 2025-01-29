using System.Linq.Expressions;
using MongoDB.Bson;

namespace CampusManagementSystem.Services;

public interface IBaseService<T> where T : class
{
    Task<T> CreateAsync(T entity);
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync(int page = 1, int pageSize = 10);
    Task<T> UpdateAsync(string id, T entity);
    Task DeleteAsync(string id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter);
    Task<long> CountAsync();
}