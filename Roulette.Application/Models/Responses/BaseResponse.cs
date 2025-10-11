namespace Roulette.Application.Models.Responses
{
    public record BaseResponse<T>(bool IsSuccess, string Code, string Message, T? Data)
    {
        public static BaseResponse<T> Success(T data, string message = "Operation completed successfully.", string code = "OK") =>
            new(true, code, message, data);

        public static BaseResponse<T> Fail(string message) =>
            new(false, AppCodes.System.INTERNAL_ERROR, message, default);
    }
}
