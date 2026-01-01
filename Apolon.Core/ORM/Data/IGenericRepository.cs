
namespace Apolon.Core.ORM.Data
{
    public interface IGenericRepository<T> where T : new()
    {
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task Update(T entity);
        Task DeleteAsync(object id);
        Task<IEnumerable<T>> GetAsync(
            string? whereClause = null, 
            object? queryParams = null, 
            string? orderBy = null
        );
    }
}