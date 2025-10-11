using Microsoft.AspNetCore.Mvc;
using Roulette.Application.Models;
using Roulette.Application.Models.Requests;
using Roulette.Application.Models.Responses.Bet;
using Roulette.Domain.Contracts.Services.Bet;

namespace Roulette.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Bet")]
    public class CreateBetController(ICreateBetService<CreateBetRequest, CreateBetResponse> createBetService) : ControllerBase
    {
        private readonly ICreateBetService<CreateBetRequest, CreateBetResponse> _createBetService = createBetService;
        [HttpPost("create")]
        public async Task<ActionResult<CreateBetResponse>> CreateBet([FromHeader(Name = "IdUser")] int userId, [FromBody] CreateBetRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(CreateBetResponse.Fail(AppCodes.System.VALIDATION_ERROR, "Invalid input data."));

            var result = await _createBetService.ExecuteAsync(userId, request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
