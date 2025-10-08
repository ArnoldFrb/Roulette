namespace Roulette.Domain.Contracts.Services.Roulette
{
    public interface IOpenRouletteService<Response>
    {
        public Task<Response> ExecuteAsync(int rouletteId);
    }
}
