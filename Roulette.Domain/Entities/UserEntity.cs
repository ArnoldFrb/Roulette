using Roulette.Domain.Entities.Base;

namespace Roulette.Domain.Entities
{
    public class UserEntity : Entity<int>
    {
        public string Username { get; protected set; } = string.Empty;

        protected UserEntity() { }
        public UserEntity(string username)
        {
            Username = username;
        }
    }
}
