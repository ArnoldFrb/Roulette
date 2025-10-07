namespace Roulette.Domain.Contracts.Services.Roulette
{
    public interface IOpenRouletteService<out Response>
    {
        public Response Execute(int rouletteId);
    }
}
