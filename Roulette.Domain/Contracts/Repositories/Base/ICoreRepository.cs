using System.Linq.Expressions;

namespace Roulette.Domain.Contracts.Repositories.Base
{
    public interface ICoreRepository<T>
    {
        T Find(object id);
        void Add(T entity);
        void Edit(T entity);
        void Remove(T entity);
        void AddRange(IEnumerable<T> entities);
        void RemoveRange(IEnumerable<T> entities);
        IEnumerable<T> GetAll();
        T FindSingleOrDefault(Expression<Func<T, bool>> predicate);
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        IEnumerable<T> FindBy(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "");
    }
}
