namespace Roulette.Domain.Contracts.Services.Bet
{
    public interface ICreateBetService<in Request, out Response>
    {
        public Response Execute(int userId, Request request);
    }
}
