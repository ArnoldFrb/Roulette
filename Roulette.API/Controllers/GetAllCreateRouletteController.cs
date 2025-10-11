using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Roulette.Application.Models.Responses.Roulette;
using Roulette.Domain.Contracts.Services.Roulette;

namespace Roulette.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Roulette")]
    public class GetAllRouletteController(IGetAllRouletteService<ListRouletteResponse> getAllRouletteService) : ControllerBase
    {
        private readonly IGetAllRouletteService<ListRouletteResponse> _getAllRouletteService = getAllRouletteService;

        [HttpGet("all")]
        public async Task<ActionResult<ListRouletteResponse>> GetAllRoulettes()
        {
            var result = await _getAllRouletteService.ExecuteAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
