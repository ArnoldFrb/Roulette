using Microsoft.AspNetCore.Mvc;
using Roulette.Application.Models;
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
            var response = await _openRouletteService.ExecuteAsync(id);

            if (!response.IsSuccess )
            {
                return response.Code switch
                {
                    AppCodes.Roulette.ROULETTE_NOT_FOUND => NotFound(response),
                    AppCodes.System.INTERNAL_ERROR => StatusCode(500, response),
                    _ => BadRequest(response),
                };
            }
            return Ok(response);
        }
    }
}
