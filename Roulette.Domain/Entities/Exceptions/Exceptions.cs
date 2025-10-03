namespace Roulette.Domain.Entities.Exceptions
{
    /// <summary>
    /// USER EXCEPTIONS
    /// </summary>
    public class InvalidUsernameException : Exception
    {
        public InvalidUsernameException() : base("Invalid username.") { }

        public InvalidUsernameException(string? message) : base(message)
        {
        }

        public InvalidUsernameException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException() : base("Invalid password.") { }

        public InvalidPasswordException(string? message) : base(message)
        {
        }

        public InvalidPasswordException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    public class InsufficientCreditsException : Exception
    {
        public InsufficientCreditsException() : base("Insufficient credits.") { }

        public InsufficientCreditsException(string? message) : base(message)
        {
        }

        public InsufficientCreditsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// ROULETTE EXCEPTIONS
    /// </summary>
    public class InvalidRouletteStatusException : Exception
    {
        public InvalidRouletteStatusException() : base("Invalid status.") { }

        public InvalidRouletteStatusException(string? message) : base(message)
        {
        }

        public InvalidRouletteStatusException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    public class InvalidNumberWinnerException : Exception
    {
        public InvalidNumberWinnerException() : base($"Invalid Number. Must be between {RouletteConstants.MinNumber} and {RouletteConstants.MaxNumber}.") { }

        public InvalidNumberWinnerException(string? message) : base(message)
        {
        }

        public InvalidNumberWinnerException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// BET EXCEPTIONS
    /// </summary>
    public class InvalidBetAmountException : Exception
    {
        public InvalidBetAmountException() : base($"Invalid amount. Must be between {RouletteConstants.MinBet} and {RouletteConstants.MaxBet}.") { }

        public InvalidBetAmountException(string? message) : base(message)
        {
        }

        public InvalidBetAmountException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
    public class InvalidBetTypeException : Exception
    {
        public InvalidBetTypeException() : base("Invalid bet type.") { }

        public InvalidBetTypeException(string message) : base(message) { }

        public InvalidBetTypeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    public class InvalidBetColorException : Exception
    {
        public InvalidBetColorException() : base("Invalid color bet. Must be 'red' or 'black'.") { }

        public InvalidBetColorException(string message) : base(message) { }

        public InvalidBetColorException(string? message, Exception? innerException) : base(message, innerException) { }
    }

    public class InvalidBetNumberException : Exception
    {
        public InvalidBetNumberException() : base($"Invalid number bet. Must be between {RouletteConstants.MinNumber} and {RouletteConstants.MaxNumber}.") { }

        public InvalidBetNumberException(string message) : base(message) { }

        public InvalidBetNumberException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
