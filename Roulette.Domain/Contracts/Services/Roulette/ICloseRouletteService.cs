namespace Roulette.Domain.Contracts.Services.Roulette
{
    public interface ICloseRouletteService<Response>
    {
         public Task<Response> ExecuteAsync(int rouletteId);
    }
}
