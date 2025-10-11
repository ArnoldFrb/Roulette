using Microsoft.AspNetCore.Mvc;
using Roulette.Application.Models;
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
            var response = await _closeRouletteService.ExecuteAsync(id);

            if (!response.IsSuccess)
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
