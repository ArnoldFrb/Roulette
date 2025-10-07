namespace Roulette.Application.Models.Responses
{
    public record AuthenticationResponse(int? Id, string? UserName, string Message)
    {
        public static AuthenticationResponse Success(int id, string userName) =>
            new(id, userName, "Authentication successful.");

        public static AuthenticationResponse Fail(string message) =>
            new(null, null, message);
    }
}
