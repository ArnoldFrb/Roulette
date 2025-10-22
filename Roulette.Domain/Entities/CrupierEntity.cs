namespace Roulette.Domain.Entities
{
    public class CrupierEntity : UserEntity
    {
        public string Password { get; protected set; } = string.Empty;

        protected CrupierEntity() { }
        public CrupierEntity(string username, string password, bool isHashed = true) : base(username)
        {
            Password = isHashed ? Hash(password) : password;
        }

        private static string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool ValidatePassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, Password);
        }
    }
}
