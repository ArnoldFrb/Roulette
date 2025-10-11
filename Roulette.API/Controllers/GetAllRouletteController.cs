using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Roulette.Application.Models;
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
            var response = await _getAllRouletteService.ExecuteAsync();
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
