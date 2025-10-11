using Microsoft.AspNetCore.Mvc;
using Roulette.Application.Models.Responses.Roulette;
using Roulette.Domain.Contracts.Services.Roulette;

namespace Roulette.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Roulette")]
    public class OpenRouletteController(IOpenRouletteService<OpenRouletteResponse> openRouletteService) : ControllerBase
    {
        private readonly IOpenRouletteService<OpenRouletteResponse> _openRouletteService = openRouletteService;

        [HttpPut("open/{id:int}")]
        public async Task<ActionResult<OpenRouletteResponse>> OpenRoulette(int id)
        {
            var result = await _openRouletteService.ExecuteAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
