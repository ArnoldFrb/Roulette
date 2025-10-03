namespace Roulette.Domain.Contracts.Services
{
    public interface IUnitOfWork
    {
        public void Commit();
    }
}
