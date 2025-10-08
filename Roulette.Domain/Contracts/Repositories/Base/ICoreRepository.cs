using System.Linq.Expressions;

namespace Roulette.Domain.Contracts.Repositories.Base
{
    public interface ICoreRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task EditAsync(T entity);
        Task RemoveAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task RemoveRangeAsync(IEnumerable<T> entities);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> FindSingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "");
    }
}
