namespace Roulette.Domain.Contracts.Services.Roulette
{
    public interface IGetAllRouletteService<out Response>
    {
        public Response Execute();
    }
}
