using FluentAssertions;
using Moq;
using Roulette.Application.Models;
using Roulette.Application.RouletteServices;
using Roulette.Domain.Contracts.Redis;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Entities;
using System.Linq.Expressions;

namespace Roulette.Application.Test
{
    public class OpenRouletteTest
    {
        private readonly Mock<IRouletteRepository> _repository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IRedisCacheService> _redis;

        private readonly RouletteEntity _roulette;
        public OpenRouletteTest()
        {
            _repository = new Mock<IRouletteRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _redis = new Mock<IRedisCacheService>();

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
            var service = new OpenRouletteService(_repository.Object, _unitOfWork.Object, _redis.Object);

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
            _repository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);
            var service = new OpenRouletteService(_repository.Object, _unitOfWork.Object, _redis.Object);

            // Act
            var response = await service.ExecuteAsync(1);

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Code.Should().Be(AppCodes.Roulette.ROULETTE_OPENED);
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
            _repository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).Throws(new Exception("DB error."));
            var service = new OpenRouletteService(_repository.Object, _unitOfWork.Object, _redis.Object);

            // Act
            var response = await service.ExecuteAsync(1);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.Code.Should().Be(AppCodes.System.INTERNAL_ERROR);
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
            _repository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);
            _repository.Setup(r => r.EditAsync(It.IsAny<RouletteEntity>())).Throws(new Exception("Update error."));
            var service = new OpenRouletteService(_repository.Object, _unitOfWork.Object, _redis.Object);

            // Act
            var response = await service.ExecuteAsync(1);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.Code.Should().Be(AppCodes.System.INTERNAL_ERROR);
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
            _repository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);
            _unitOfWork.Setup(u => u.CommitTransactionAsync()).Throws(new Exception("Transaction error."));
            var service = new OpenRouletteService(_repository.Object, _unitOfWork.Object, _redis.Object);

            // Act
            var response = await service.ExecuteAsync(1);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.Code.Should().Be(AppCodes.System.INTERNAL_ERROR);
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
            _repository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);
            var service = new OpenRouletteService(_repository.Object, _unitOfWork.Object, _redis.Object);

            // Act
            await service.ExecuteAsync(1);

            // Assert
            _repository.Verify(r => r.EditAsync(It.IsAny<RouletteEntity>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }
    }
}