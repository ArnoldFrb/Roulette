using Roulette.Domain.Contracts.Entities;

namespace Roulette.Domain.Entities.Base
{
    public interface IBaseEntity;

    public abstract class Entity<T> : IBaseEntity, IEntity<T>
    {
        public required T Id { get; set; }
    }
}
