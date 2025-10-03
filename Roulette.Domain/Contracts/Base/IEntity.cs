namespace Roulette.Domain.Contracts.Base
{
    public interface IEntity<T>
    {
        T? Id { get; set; }
    }
}
