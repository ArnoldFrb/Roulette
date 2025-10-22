using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Roulette.API.Test.Utils;
using Roulette.Application.Models.Requests;
using Roulette.Application.Models.Responses.User;
using Roulette.Infrastructure.Data;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Roulette.API.Test
{
    public class AuthenticationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthenticationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();

            var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RouletteDbContext>();
            db.Database.EnsureCreated();
        }

        /*
         1.	Usuario correcto
            •	Dado usuario "crupier1" y contraseña "password1"
            •	Cuando se llama a la api "api/auth"
            •	Entonces el servicio debe responder 200 OK y data no debe ser null
        */
        [Fact]
        public async Task Authenticate_ReturnsJwtToken()
        {
            // Arrange
            var request = new AuthenticationRequest("crupier1", "password1");

            // Act
            var response = await _client.PostAsJsonAsync("api/auth", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<CrupierResponse>();
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
        }

        /*
         2.	Credenciales Vacías
            •	Dado usuarios o contraseñas vacías
            •	Cuando se llama a la api "api/auth"
            •	Entonces el servicio debe responder 400 BadRequest
        */
        [Theory]
        [InlineData("", "")]
        [InlineData("user1", "")]
        [InlineData("", "user1pass")]
        public async Task Authenticate_ReturnsUnauthorized_WhenCredentialsEmpty(string user, string pass)
        {
            // Arrange
            var request = new AuthenticationRequest(user, pass);

            // Act
            var response = await _client.PostAsJsonAsync("api/auth", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /*
         3.	Credenciales Invalidas
            •	Dado un usuario o contraseña incorrecta
            •	Cuando se llama a la api "api/auth"
            •	Entonces el servicio debe responder 400
        */
        [Fact]
        public async Task Authenticate_ReturnsUnauthorized_WhenCredentialsInvalid()
        {
            // Arrange
            var request = new AuthenticationRequest("crupier1", "user1pass");

            // Act
            var response = await _client.PostAsJsonAsync("api/auth", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}