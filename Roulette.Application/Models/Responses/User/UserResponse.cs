namespace Roulette.Application.Models.Responses.User
{
    public record UserDto(int? Id, string? UserName);
    public record UserResponse : BaseResponse<UserDto>
    {
        private UserResponse(bool IsSuccess, string Code, string Message, UserDto? Data) : base(IsSuccess, Code, Message, Data)
        {
        }

        public static UserResponse Success(UserDto data) =>
            new(true, AppCodes.Auth.AUTH_SUCCESS, "Authentication successful.", data);

        public static UserResponse Fail(string code, string message) =>
            new(false, code, $"Authentication failed: {message}", null);
    }
}
