using Microsoft.AspNetCore.Mvc;
using Roulette.Application.Models.Responses.Roulette;
using Roulette.Domain.Contracts.Services.Roulette;

namespace Roulette.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Roulette")]
    public class CloseRouletteController(ICloseRouletteService<CloseRouletteResponse> closeRouletteService) : ControllerBase
    {
        private readonly ICloseRouletteService<CloseRouletteResponse> _closeRouletteService = closeRouletteService;

        [HttpPut("close/{id:int}")]
        public async Task<ActionResult<CloseRouletteResponse>> CloseRoulette(int id)
        {
            var result = await _closeRouletteService.ExecuteAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
