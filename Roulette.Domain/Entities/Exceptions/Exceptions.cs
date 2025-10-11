namespace Roulette.Domain.Entities.Exceptions
{
    /// <summary>
    /// USER EXCEPTIONS
    /// </summary>
    public class InvalidUsernameOrPasswordException : Exception
    {
        public InvalidUsernameOrPasswordException() : base("The credentials are incorrect.") { }

        public InvalidUsernameOrPasswordException(string? message) : base(message)
        {
        }

        public InvalidUsernameOrPasswordException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    public class InvalidCreditOperationException : Exception
    {
        public InvalidCreditOperationException() : base("Insufficient credits.") { }

        public InvalidCreditOperationException(string? message) : base(message)
        {
        }

        public InvalidCreditOperationException(string? message, Exception? innerException) : base(message, innerException)
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
        public InvalidBetAmountException() : base($"Bet amount must be between {RouletteConstants.MinBet:C} and {RouletteConstants.MaxBet:C}.") { }

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
