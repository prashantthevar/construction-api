using System.Linq.Expressions;

namespace ConstructionManagementSaaS.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task AddAsync(T entity);
        Task UpdateAsync(string id, T entity);
        Task DeleteAsync(string id);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> filter);
    }
}
