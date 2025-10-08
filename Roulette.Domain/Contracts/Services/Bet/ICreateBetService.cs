namespace Roulette.Domain.Contracts.Services.Bet
{
    public interface ICreateBetService<in Request, Response>
    {
        public Task<Response> ExecuteAsync(int userId, Request request);
    }
}
