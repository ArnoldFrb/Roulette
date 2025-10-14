namespace Roulette.Application.Models.Responses.User
{
    public record GamblerDto(int Id, string Username, decimal Credits);
    public record GamblerResponse : BaseResponse<GamblerDto>
    {
        private GamblerResponse(bool IsSuccess, string Code, string Message, GamblerDto? Data) : base(IsSuccess, Code, Message, Data)
        {
        }

        public static GamblerResponse Success(GamblerDto data) =>
            new(true, AppCodes.Auth.AUTH_SUCCESS, "Authentication successful.", data);

        public static GamblerResponse Fail(string code, string message) =>
            new(false, code, $"Authentication failed: {message}", null);
    }
}
