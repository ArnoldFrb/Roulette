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
    public class GetAllRouletteTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public GetAllRouletteTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();

            var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RouletteDbContext>();
            db.Database.EnsureCreated();
        }

        /*
         1.	Usuario no autorizado
            •	Dado que se desea crear un ruleta y el usuario no esta autentificado
            •	Cuando se llama a la api "api/roulette/all"
            •	Entonces el servicio debe responder 401 Unauthorized
        */
        [Fact]
        public async Task GetAllRoulette_Unauthorized_WithoutToken()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

            // Act
            var response = await _client.GetAsync("api/roulette/all");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        /*
         2.	Obtener ruletas
            •	Dado que se desea obtener las ruletas y el usuario esta autentificado
            •	Cuando se llama a la api "api/roulette/all"
            •	Entonces el servicio debe responder 200 OK
        */
        [Fact]
        public async Task GetAllRoulette_ReturnsOk_WithToken()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CustomWebApplicationFactory.GenerateValidToken());

            // Act
            var response = await _client.GetAsync("api/roulette/all");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ListRouletteResponse>();
            result.Should().NotBeNull();
            result.Code.Should().Be(AppCodes.Roulette.ROULETTE_LISTED);
        }
    }
}
