namespace Roulette.Domain.Contracts.Services.User
{
    public interface IGetGamblerService<Response>
    {
        public Task<Response> ExecuteAsync(string username);
    }
}
