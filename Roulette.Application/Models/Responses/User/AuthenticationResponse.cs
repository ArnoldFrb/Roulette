namespace Roulette.Application.Models.Responses.User
{
    public record AuthenticationDto(int? Id, string? UserName);
    public record AuthenticationResponse : BaseResponse<AuthenticationDto>
    {
        private AuthenticationResponse(bool IsSuccess, string Code, string Message, AuthenticationDto? Data) : base(IsSuccess, Code, Message, Data)
        {
        }

        public static AuthenticationResponse Success(AuthenticationDto data) =>
            new(true, AppCodes.Auth.AUTH_SUCCESS, "Authentication successful.", data);

        public static AuthenticationResponse Fail(string code, string message) =>
            new(false, code, $"Authentication failed: {message}", null);
    }
}
