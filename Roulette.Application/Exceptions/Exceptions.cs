namespace Roulette.Application.Exceptions
{
    /// <summary>
    /// USER EXCEPTIONS
    /// </summary>
    public class AuthenticationException : Exception
    {
        public AuthenticationException() : base("User not found") { }

        public AuthenticationException(string? message) : base(message)
        {
        }

        public AuthenticationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
