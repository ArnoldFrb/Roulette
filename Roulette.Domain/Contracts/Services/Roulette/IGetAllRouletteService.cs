namespace Roulette.Domain.Contracts.Services.Roulette
{
    public interface IGetAllRouletteService<Response>
    {
        public Task<Response> ExecuteAsync();
    }
}
