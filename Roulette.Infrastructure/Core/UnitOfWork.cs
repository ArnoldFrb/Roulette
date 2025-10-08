using Microsoft.EntityFrameworkCore.Storage;
using Roulette.Domain.Contracts.Services;

namespace Roulette.Infrastructure.Core
{
    public class UnitOfWork(RouletteDbContext context) : IUnitOfWork
    {
        private readonly RouletteDbContext _context = context;
        private IDbContextTransaction? _transaction;

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            await _context.DisposeAsync();
        }

        public async Task RollbackTransactionAsync()
        {
           if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
