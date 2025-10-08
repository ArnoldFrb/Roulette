namespace Roulette.Domain.Contracts.Services.Roulette
{
    public interface ICreateRouletteService<Response>
    {
        public Task<Response> ExecuteAsync();
    }
}
