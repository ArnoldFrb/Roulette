namespace Roulette.Domain.Contracts.Services.User
{
    public interface IAuthenticationService<in Request, Response>
    {
        public Task<Response> ExecuteAsync(Request request);
    }
}
