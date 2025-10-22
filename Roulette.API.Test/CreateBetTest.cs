using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Roulette.API.Test.Utils;
using Roulette.Application.Models;
using Roulette.Application.Models.Requests;
using Roulette.Application.Models.Responses.Bet;
using Roulette.Application.Models.Responses.Roulette;
using Roulette.Domain.Entities;
using Roulette.Infrastructure.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace Roulette.API.Test
{
    public class CreateBetTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly string _token;

        public CreateBetTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _token = CustomWebApplicationFactory.GenerateValidToken();

            var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RouletteDbContext>();
            db.Database.EnsureCreated();
        }

        /*
         1.	Usuario no existe
            •	Dado que se desea crear una apuesta y el usuario no existe
            •	Cuando se llama a la api "api/bet/create"
            •	Entonces el servicio debe responder 404 Not Found
        */
        [Fact]
        public async Task OpenRoulette_ReturnsError_UserNotExist()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("IdUser", "1000");
            var request = new CreateBetRequest(100, BetType.Number, "10", 1);

            // Act
            var response = await _client.PostAsJsonAsync("api/bet/create", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var result = await response.Content.ReadFromJsonAsync<CreateBetResponse>();
            result.Should().NotBeNull();
            result.Code.Should().Be(AppCodes.User.USER_NOT_FOUND);
        }

        /*
         2.	Ruleta no existe
            •	Dado que se desea crear una apuesta y la ruleta no existe
            •	Cuando se llama a la api "api/bet/create"
            •	Entonces el servicio debe responder 404 Not Found
        */
        [Fact]
        public async Task OpenRoulette_ReturnsError_RouletteNotExist()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("IdUser", "2");

            var request = new CreateBetRequest(100, BetType.Number, "10", 1000);

            // Act
            var response = await _client.PostAsJsonAsync("api/bet/create", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var result = await response.Content.ReadFromJsonAsync<CreateBetResponse>();
            result.Should().NotBeNull();
            result.Code.Should().Be(AppCodes.Roulette.ROULETTE_NOT_FOUND);
        }

        /*
         3.	Ruleta no esta abierta
            •	Dado que se desea crear una apuesta y la ruleta no esta abierta
            •	Cuando se llama a la api "api/bet/create"
            •	Entonces el servicio debe responder 500 InternalServerError
        */
        [Fact]
        public async Task OpenRoulette_ReturnsError_RouletteIsNotOpen()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("IdUser", "2");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var responseRoulette = await _client.PostAsync("api/roulette/create", null);
            var roulette = await responseRoulette.Content.ReadFromJsonAsync<CloseRouletteResponse>();

            var request = new CreateBetRequest(100, BetType.Number, "10", (int)roulette!.Data!.Id!);

            // Act
            var response = await _client.PostAsJsonAsync("api/bet/create", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            var result = await response.Content.ReadFromJsonAsync<CreateBetResponse>();
            result.Should().NotBeNull();
            result.Code.Should().Be(AppCodes.Bet.BET_CREATION_ERROR);
        }

        /*
         4.	Ruleta esta cerrada
            •	Dado que se desea crear una apuesta y la ruleta esta cerrada
            •	Cuando se llama a la api "api/bet/create"
            •	Entonces el servicio debe responder 500 InternalServerError
        */
        [Fact]
        public async Task OpenRoulette_ReturnsError_RouletteIsClose()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("IdUser", "2");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var responseRoulette = await _client.PostAsync("api/roulette/create", null);
            var roulette = await responseRoulette.Content.ReadFromJsonAsync<CloseRouletteResponse>();
            await _client.PutAsync($"api/roulette/open/{roulette!.Data!.Id}", null);
            await _client.PutAsync($"api/roulette/close/{roulette!.Data!.Id}", null);

            var request = new CreateBetRequest(100, BetType.Number, "10", (int)roulette!.Data!.Id!);

            // Act
            var response = await _client.PostAsJsonAsync("api/bet/create", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            var result = await response.Content.ReadFromJsonAsync<CreateBetResponse>();
            result.Should().NotBeNull();
            result.Code.Should().Be(AppCodes.Bet.BET_CREATION_ERROR);
        }

        /*
         5.	Crear apuesta
            •	Dado que se desea crear una apuesta y los datos son incorrectos
            •	Cuando se llama a la api "api/roulette/open"
            •	Entonces el servicio debe responder 200 OK
        */
        [Theory]
        [InlineData(-1, BetType.Number, "15")]
        [InlineData(0, BetType.Number, "15")]
        [InlineData(100, BetType.Number, "-1")]
        [InlineData(100, BetType.Number, "-45")]
        [InlineData(100, BetType.Color, "-45")]
        [InlineData(100, BetType.Color, "Blue")]
        [InlineData(10001, BetType.Color, "Red")]
        public async Task OpenRoulette_ReturnsError_DataInvalid(decimal amount, BetType type, string value)
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("IdUser", "2");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var responseRoulette = await _client.PostAsync("api/roulette/create", null);
            var roulette = await responseRoulette.Content.ReadFromJsonAsync<CloseRouletteResponse>();
            await _client.PutAsync($"api/roulette/open/{roulette!.Data!.Id}", null);

            var request = new CreateBetRequest(amount, type, value, (int)roulette!.Data!.Id!);

            // Act
            var response = await _client.PostAsJsonAsync("api/bet/create", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadFromJsonAsync<OpenRouletteResponse>();
            result.Should().NotBeNull();
        }

        /*
         6.	Crear apuesta para número
            •	Dado que se desea crear una apuesta y el numero es 15
            •	Cuando se llama a la api "api/bet/create"
            •	Entonces el servicio debe responder 200 Ok
        */
        [Fact]
        public async Task OpenRoulette_ReturnsOk_BetTypeNumber()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("IdUser", "2");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var responseRoulette = await _client.PostAsync("api/roulette/create", null);
            var roulette = await responseRoulette.Content.ReadFromJsonAsync<CloseRouletteResponse>();
            await _client.PutAsync($"api/roulette/open/{roulette!.Data!.Id}", null);

            var request = new CreateBetRequest(100, BetType.Number, "15", (int)roulette!.Data!.Id!);

            // Act
            var response = await _client.PostAsJsonAsync("api/bet/create", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<CreateBetResponse>();
            result.Should().NotBeNull();
            result.Code.Should().Be(AppCodes.Bet.BET_CREATED);
        }

        /*
         7.	Crear apuesta para color
            •	Dado que se desea crear una apuesta y el color es Red
            •	Cuando se llama a la api "api/bet/create"
            •	Entonces el servicio debe responder 200 Ok
        */
        [Fact]
        public async Task OpenRoulette_ReturnsOk_BetTypeColor()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("IdUser", "2");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var responseRoulette = await _client.PostAsync("api/roulette/create", null);
            var roulette = await responseRoulette.Content.ReadFromJsonAsync<CloseRouletteResponse>();
            await _client.PutAsync($"api/roulette/open/{roulette!.Data!.Id}", null);

            var request = new CreateBetRequest(100, BetType.Color, "red", (int)roulette!.Data!.Id!);

            // Act
            var response = await _client.PostAsJsonAsync("api/bet/create", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<CreateBetResponse>();
            result.Should().NotBeNull();
            result.Code.Should().Be(AppCodes.Bet.BET_CREATED);
        }

        /*
         8.	Crear dos apuestas en la misma ruleta
            •	Dado que se desea crear una apuesta y ya existe una apuesta en la ruleta
            •	Cuando se llama a la api "api/bet/create"
            •	Entonces el servicio debe responder 500 InternalServerError
        */
        [Fact]
        public async Task OpenRoulette_ReturnsOk_BetExist()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("IdUser", "2");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var responseRoulette = await _client.PostAsync("api/roulette/create", null);
            var roulette = await responseRoulette.Content.ReadFromJsonAsync<CloseRouletteResponse>();
            await _client.PutAsync($"api/roulette/open/{roulette!.Data!.Id}", null);

            var request = new CreateBetRequest(100, BetType.Color, "red", (int)roulette!.Data!.Id!);
            await _client.PostAsJsonAsync("api/bet/create", request);

            // Act
            var response = await _client.PostAsJsonAsync("api/bet/create", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            var result = await response.Content.ReadFromJsonAsync<CreateBetResponse>();
            result.Should().NotBeNull();
            result.Code.Should().Be(AppCodes.Bet.BET_CREATION_ERROR);
        }
    }
}
