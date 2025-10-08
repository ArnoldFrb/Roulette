using FluentAssertions;
using Moq;
using Roulette.Application.RouletteServices;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Entities;
using System.Linq.Expressions;

namespace Roulette.Application.Test
{
    public class OpenRouletteTest
    {
        private readonly RouletteEntity _roulette;
        public OpenRouletteTest()
        {
            _roulette = new RouletteEntity() { Id = 1 };
        }

        /*
         1.	Debe devolver error si la ruleta no existe
            •	Dado una una petición para abrir una ruleta con Id 5 que no existe en la base de datos
            •	Cuando se llama al método Execute del servicio OpenRoulette
            •	Entonces se debe devolver un RouletteResponse con Id null, Status null, Date null y Message "Error opening roulette: Roulette not found."
        */
        [Fact]
        [Trait("Category", "OpenRoulette")]
        public async Task Execute_WhenRouletteNotFound_ShouldReturnErrorResponse()
        {
            // Arrange
            var repository = new Mock<IRouletteRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var service = new OpenRouletteService(repository.Object, unitOfWork.Object);

            // Act
            var response = await service.ExecuteAsync(5);

            // Assert
            response.Message.Should().Be("Error opening roulette: Roulette not found.");
        }

        /*
         2.	Debe abrir la ruleta correctamente
            •	Dado una una petición para abrir una ruleta con Id 1 que existe en la base de datos
            •	Cuando se llama al método Execute del servicio OpenRoulette
            •	Entonces se debe devolver un RouletteResponse con Id no null, Status "Open", Date fecha actual de apertura y Message "Roulette opened successfully."
        */
        [Fact]
        [Trait("Category", "OpenRoulette")]
        public async Task Execute_WhenRouletteExists_ShouldOpenSuccessfully()
        {
            // Arrange
            var repository = new Mock<IRouletteRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            repository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);
            var service = new OpenRouletteService(repository.Object, unitOfWork.Object);

            // Act
            var response = await service.ExecuteAsync(1);

            // Assert
            response.Id.Should().Be(1);
            response.Status.Should().Be("Open");
            response.OpenedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            response.Message.Should().Be("Roulette opened successfully.");
        }

        /*
         3.	Debe devolver error si FindSingleOrDefault lanza excepción
            •	Dado una una petición para abrir una ruleta y FindSingleOrDefault lanza una excepción
            •	Cuando se llama al método Execute del servicio OpenRoulette
            •	Entonces se debe devolver un RouletteResponse con Id null, Status null, Date null y Message "DB error."
        */
        [Fact]
        [Trait("Category", "OpenRoulette")]
        public async Task Execute_WhenFindThrowsException_ShouldReturnErrorResponse()
        {
            // Arrange
            var repository = new Mock<IRouletteRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            repository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).Throws(new Exception("DB error."));
            var service = new OpenRouletteService(repository.Object, unitOfWork.Object);

            // Act
            var response = await service.ExecuteAsync(1);

            // Assert
            response.Id.Should().BeNull();
            response.Status.Should().BeNull();
            response.OpenedAt.Should().BeNull();
            response.Message.Should().Be("Error opening roulette: DB error.");
        }

        /*
         4.	Debe devolver error si Edit lanza excepción
            •	Dado una una petición para abrir una ruleta y Edit lanza una excepción
            •	Cuando se llama al método Execute del servicio OpenRoulette
            •	Entonces se debe devolver un RouletteResponse con Id null, Status null, Date null y Message "DB error."
        */
        [Fact]
        [Trait("Category", "OpenRoulette")]
        public async Task Execute_WhenEditThrowsException_ShouldReturnErrorResponse()
        {
            // Arrange
            var repository = new Mock<IRouletteRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            repository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);
            repository.Setup(r => r.EditAsync(It.IsAny<RouletteEntity>())).Throws(new Exception("Update error."));
            var service = new OpenRouletteService(repository.Object, unitOfWork.Object);

            // Act
            var response = await service.ExecuteAsync(1);

            // Assert
            response.Id.Should().BeNull();
            response.Status.Should().BeNull();
            response.OpenedAt.Should().BeNull();
            response.Message.Should().Be("Error opening roulette: Update error.");
        }

        /*
         5.	Debe devolver error si Commit lanza excepción
            •	Dado una una petición para abrir una ruleta y Commit lanza una excepción
            •	Cuando se llama al método Execute del servicio OpenRoulette
            •	Entonces se debe devolver un RouletteResponse con Id null, Status null, Date null y Message "Transaction error."
        */
        [Fact]
        [Trait("Category", "OpenRoulette")]
        public async Task Execute_WhenCommitThrowsException_ShouldReturnErrorResponse()
        {
            // Arrange
            var repository = new Mock<IRouletteRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            repository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);
            unitOfWork.Setup(u => u.CommitTransactionAsync()).Throws(new Exception("Transaction error."));
            var service = new OpenRouletteService(repository.Object, unitOfWork.Object);

            // Act
            var response = await service.ExecuteAsync(1);

            // Assert
            response.Id.Should().BeNull();
            response.Status.Should().BeNull();
            response.OpenedAt.Should().BeNull();
            response.Message.Should().Be("Error opening roulette: Transaction error.");
        }

        /*
         6.	Debe llamar a Edit y Commit exactamente una vez cuando la ruleta existe
            •	Dado una una petición para abrir una ruleta y Commit lanza una excepción
            •	Cuando se llama al método Execute del servicio OpenRoulette
            •	Entonces se debe devolver un RouletteResponse con Id null, Status null, Date null y Message "Transaction error."
        */
        [Fact]
        [Trait("Category", "OpenRoulette")]
        public async Task Execute_ShouldCallEditAndCommitOnce()
        {
            // Arrange
            var repository = new Mock<IRouletteRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            repository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);
            var service = new OpenRouletteService(repository.Object, unitOfWork.Object);

            // Act
            await service.ExecuteAsync(1);

            // Assert
            repository.Verify(r => r.EditAsync(It.IsAny<RouletteEntity>()), Times.Once);
            unitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }
    }
}