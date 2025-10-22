using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Roulette.API.Test.Utils;
using Roulette.Application.Models;
using Roulette.Application.Models.Responses.Roulette;
using Roulette.Infrastructure.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace Roulette.API.Test
{
    public class OpenRouletteTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly string _token;

        public OpenRouletteTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _token = CustomWebApplicationFactory.GenerateValidToken();

            var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RouletteDbContext>();
            db.Database.EnsureCreated();
        }

        /*
         1.	Usuario no autorizado
            •	Dado que se desea abrir un ruleta y el usuario no esta autentificado
            •	Cuando se llama a la api "api/roulette/open"
            •	Entonces el servicio debe responder 401 Unauthorized
        */
        [Fact]
        public async Task OpenRoulette_Unauthorized_WithoutToken()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

            // Act
            var response = await _client.PutAsync("api/roulette/open/1", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        /*
         2.	Abrir ruleta
            •	Dado que se desea abrir un ruleta y el usuario esta autentificado
            •	Cuando se llama a la api "api/roulette/open"
            •	Entonces el servicio debe responder 200 OK
        */
        [Fact]
        public async Task OpenRoulette_ReturnsOk_WithToken()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var responseRoulette = await _client.PostAsync("api/roulette/create", null);
            var roulette = await responseRoulette.Content.ReadFromJsonAsync<CloseRouletteResponse>();

            // Act
            var response = await _client.PutAsync($"api/roulette/open/{roulette!.Data!.Id}", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<OpenRouletteResponse>();
            result.Should().NotBeNull();
            result.Code.Should().Be(AppCodes.Roulette.ROULETTE_OPENED);
        }

        /*
         3.	Abrir ruleta que esta abierta
            •	Dado que se desea abrir un ruleta y la ruleta ya esta abierta
            •	Cuando se llama a la api "api/roulette/open"
            •	Entonces el servicio debe responder 200 OK
        */
        [Fact]
        public async Task OpenRoulette_ReturnsError_IsOpen()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var responseRoulette = await _client.PostAsync("api/roulette/create", null);
            var roulette = await responseRoulette.Content.ReadFromJsonAsync<CloseRouletteResponse>();
            await _client.PutAsync($"api/roulette/open/{roulette!.Data!.Id}", null);

            // Act
            var response = await _client.PutAsync($"api/roulette/open/{roulette!.Data!.Id}", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            var result = await response.Content.ReadFromJsonAsync<OpenRouletteResponse>();
            result.Should().NotBeNull();
            result.Code.Should().Be(AppCodes.Roulette.ROULETTE_OPEN_ERROR);
        }

        /*
         4.	Abrir ruleta que no existe
            •	Dado que se desea abrir un ruleta y la ruleta no existe
            •	Cuando se llama a la api "api/roulette/open"
            •	Entonces el servicio debe responder 404 Not Found
        */
        [Fact]
        public async Task OpenRoulette_ReturnsError_NoExist()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            // Act
            var response = await _client.PutAsync("api/roulette/open/200", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var result = await response.Content.ReadFromJsonAsync<CreateRouletteResponse>();
            result.Should().NotBeNull();
            result.Data.Should().BeNull();
        }
    }
}
