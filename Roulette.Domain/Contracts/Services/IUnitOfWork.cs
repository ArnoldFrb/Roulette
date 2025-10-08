namespace Roulette.Domain.Contracts.Services
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task CommitAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
