namespace Roulette.Domain.Contracts.Services.Roulette
{
    public interface ICloseRouletteService<out Response>
    {
         public Response Execute(int rouletteId);
    }
}
