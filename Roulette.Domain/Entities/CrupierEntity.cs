using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Entities
{
    public class CrupierEntity : UserEntity
    {
        public string Password { get; protected set; } = string.Empty;

        protected CrupierEntity() { }
        public CrupierEntity(string username, string password) : base(username)
        {
            Password = password;
        }

        public bool ValidatePassword(string password) => Password.Equals(password);

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidUsernameOrPasswordException();
            return true;
        }
    }
}
