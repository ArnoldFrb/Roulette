using Microsoft.AspNetCore.Mvc;
using Roulette.Application.Models.Responses.Roulette;
using Roulette.Domain.Contracts.Services.Roulette;

namespace Roulette.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Roulette")]
    public class CreateRouletteController(ICreateRouletteService<CreateRouletteResponse> createRouletteService) : ControllerBase
    {
        private readonly ICreateRouletteService<CreateRouletteResponse> _createRouletteService = createRouletteService;

        [HttpPost("create")]
        public async Task<ActionResult<CreateRouletteResponse>> CreateRoulette()
        {
            var response = await _createRouletteService.ExecuteAsync();
            return response.IsSuccess ? Ok(response) : StatusCode(500, response);
        }
    }
}
