using Roulette.Domain.Contracts.Base;

namespace Roulette.Domain.Entities.Base
{
    public interface IBaseEntity;

    public abstract class Entity<T> : IBaseEntity, IEntity<T>
    {
        public virtual T? Id { get; set; }
    }
}
