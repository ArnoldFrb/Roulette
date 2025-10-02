namespace Roulette.Domain.Entities.Exceptions
{
    public class GenericException : Exception
    {
        public GenericException() : base("404 Not Found") { }

        public GenericException(string? message) : base(message)
        {
        }

        public GenericException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
