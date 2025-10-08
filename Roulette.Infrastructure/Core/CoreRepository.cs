using Microsoft.EntityFrameworkCore;
using Roulette.Domain.Contracts.Repositories.Base;
using System.Linq.Expressions;

namespace Roulette.Infrastructure.Core
{
    public class CoreRepository<T>(RouletteDbContext context) : ICoreRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities) => await _dbSet.AddRangeAsync(entities);

        public Task EditAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();

        public async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "") => await Task.Run(async () =>
        {
            IQueryable<T> query = _dbSet;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            foreach (var includeProperty in includeProperties.Split([','], StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();

        });

        public async Task<T?> FindSingleOrDefaultAsync(Expression<Func<T, bool>> predicate) => await _dbSet.SingleOrDefaultAsync(predicate);

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public Task RemoveAsync(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            return Task.CompletedTask;
        }
    }
}
