namespace Roulette.Domain.Contracts.Services
{
    public interface IUnitOfWork
    {
        public void BeginTransaction();
        public void CommitTransaction();
        public void RollbackTransaction();
    }
}
