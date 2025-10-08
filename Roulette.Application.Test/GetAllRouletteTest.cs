using FluentAssertions;
using Moq;
using Roulette.Application.RouletteServices;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Entities;

namespace Roulette.Application.Test
{
    public class GetAllRouletteTest
    {
        private readonly IEnumerable<RouletteEntity> _roulettes;
        private readonly Mock<IRouletteRepository> _repository;

        public GetAllRouletteTest()
        {
            _repository = new Mock<IRouletteRepository>();

            _roulettes = [
                new RouletteEntity() { Id = 1 },
                new RouletteEntity() { Id = 2 },
                new RouletteEntity() { Id = 3 },
                new RouletteEntity() { Id = 4 },
                new RouletteEntity() { Id = 5 }
                ];
        }

        /*
         1.	Existen múltiples ruletas registradas
            •	Dado una lista de ruletas en el repositorio
            •	Cuando se ejecuta el método Execute()
            •	Entonces se debe devolver un RouletteListResponse con la lista de ruletas y Message "Roulettes retrieved successfully."
        */
        [Fact]
        [Trait("Category", "GetAllRoulette")]
        public async Task Execute_ShouldReturnListOfRoulettes_WhenRoulettesExist()
        {
            // Arrange
            _repository.Setup(r => r.GetAllAsync()).ReturnsAsync(_roulettes);
            var service = new GetAllRouletteService(_repository.Object);

            // Act
            var response = await service.ExecuteAsync();

            // Assert
            response.Roulettes.Should().NotBeEmpty();
            response.Message.Should().Be("Roulettes retrieved successfully.");
        }

        /*
         2.	No hay ruletas registradas
            •	Dado que no existen ruletas registradas
            •	Cuando se ejecuta el método Execute()
            •	Entonces se debe devolver un RouletteListResponse con una lista vacía y Message "Error retrieving roulettes: No roulettes found."
        */
        [Fact]
        [Trait("Category", "GetAllRoulette")]
        public async Task Execute_ShouldReturnEmptyList_WhenNoRoulettesExist()
        {
            // Arrange
            _repository.Setup(r => r.GetAllAsync()).ReturnsAsync([]);
            var service = new GetAllRouletteService(_repository.Object);

            // Act
            var response = await service.ExecuteAsync();

            // Assert
            response.Roulettes.Should().BeEmpty();
            response.Message.Should().Be("Error retrieving roulettes: No roulettes found.");
        }

        /*
         3.	Excepción al recuperar las ruletas
            •	Dado que ocurre una excepción en el repositorio
            •	Cuando se ejecuta el método Execute()
            •	Entonces se debe devolver un RouletteListResponse con una lista vacía y Message "Error retrieving roulettes: Db Error"
        */
        [Fact]
        [Trait("Category", "GetAllRoulette")]
        public async Task Execute_ShouldReturnErrorResponse_WhenRepositoryThrowsException()
        {
            // Arrange
            _repository.Setup(r => r.GetAllAsync()).Throws(new Exception("Db Error"));
            var service = new GetAllRouletteService(_repository.Object);

            // Act
            var response = await service.ExecuteAsync();

            // Assert
            response.Roulettes.Should().BeEmpty();
            response.Message.Should().Be("Error retrieving roulettes: Db Error");
        }
    }
}
